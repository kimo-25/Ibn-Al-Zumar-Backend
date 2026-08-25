using System.Text.RegularExpressions;
using IbnAlZumar.API.DTOs.Ai;
using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.API.Services.Catalog;
using Microsoft.EntityFrameworkCore;
using Services.Sales;

namespace IbnAlZumar.API.Services.Ai
{
    /// <summary>
    /// يحوّل نص أمر صوتي عربي (مُحوَّل من صوت لنص في الفرونت إند) إلى عملية فعلية
    /// في النظام: إنشاء فاتورة بيع، أو إضافة منتج جديد.
    ///
    /// المعالجة قائمة على Regex/Heuristics محلية بالكامل - لا تعتمد على أي API خارجي،
    /// وبالتالي لا تتأثر بانقطاع خدمات مثل Hugging Face. طريقة العمل مصممة لتوسّع
    /// تدريجي: لو احتجت لاحقاً فهم أدق للجمل يمكنك استبدال/تعزيز ParseCommand
    /// باستدعاء نموذج نصي (Text Generation) مجاني على Hugging Face كطبقة تحسين
    /// اختيارية فقط، مع الإبقاء على هذا المسار كخط رجوع (fallback) دائم يعمل بدون إنترنت.
    ///
    /// ⚠️ ملاحظات على الافتراضات:
    /// - نفترض أن كيان Product يحتوي: Id, Name, NameAr, SellingPrice, IsActive.
    /// - نفترض أن كيان Customer يحتوي: Id, FullName, Email, Phone.
    /// - عدّل أسماء الخصائص أدناه إذا كانت مختلفة فعلياً في مشروعك.
    /// </summary>
    public class VoiceCommandService : IVoiceCommandService
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        private static readonly Dictionary<string, int> ArabicNumberWords = new()
        {
            ["واحد"] = 1,
            ["واحده"] = 1,
            ["اتنين"] = 2,
            ["اثنين"] = 2,
            ["اثنان"] = 2,
            ["تلاتة"] = 3,
            ["ثلاثة"] = 3,
            ["ثلاثه"] = 3,
            ["اربعة"] = 4,
            ["أربعة"] = 4,
            ["اربعه"] = 4,
            ["خمسة"] = 5,
            ["خمسه"] = 5,
            ["ستة"] = 6,
            ["سته"] = 6,
            ["سبعة"] = 7,
            ["سبعه"] = 7,
            ["تمانية"] = 8,
            ["ثمانية"] = 8,
            ["تمانيه"] = 8,
            ["تسعة"] = 9,
            ["تسعه"] = 9,
            ["عشرة"] = 10,
            ["عشره"] = 10
        };

        public VoiceCommandService(ApplicationDbContext context, IOrderService orderService, IProductService productService)
        {
            _context = context;
            _orderService = orderService;
            _productService = productService;
        }

        public async Task<VoiceCommandResultDto> ProcessCommandAsync(string text, string? actingUserEmail, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new VoiceCommandResultDto
                {
                    Success = false,
                    Action = "Unknown",
                    Message = "لم يتم استلام أي نص للأمر الصوتي."
                };
            }

            var parsed = ParseCommand(text);

            return parsed.Intent switch
            {
                "CreateInvoice" => await HandleCreateInvoiceAsync(parsed, cancellationToken),
                "AddProduct" => await HandleAddProductAsync(parsed, cancellationToken),
                _ => new VoiceCommandResultDto
                {
                    Success = false,
                    Action = "Unknown",
                    Message = "لم أتمكن من فهم الأمر. جرّب مثلاً: \"اعمل فاتورة بيع لعميل اسمه أحمد منتج صابون بكمية 3\".",
                    ParsedCommand = parsed
                }
            };
        }

        // =========================================================
        //                  Intent Parsing
        // =========================================================

        public ParsedVoiceCommandDto ParseCommand(string rawText)
        {
            var text = rawText.Trim();
            var result = new ParsedVoiceCommandDto { RawText = rawText };

            // 1) تحديد نوع العملية
            if (Regex.IsMatch(text, @"فاتورة|بيع|اطلب|طلب"))
            {
                result.Intent = "CreateInvoice";
            }
            else if (Regex.IsMatch(text, @"(ضيف|أضف|اضافة|إضافة).*منتج|منتج.*(ضيف|أضف|اضافة|إضافة)"))
            {
                result.Intent = "AddProduct";
            }
            else
            {
                result.Intent = "Unknown";
                return result;
            }

            // 2) استخراج اسم العميل: بعد "اسمه/اسمها/لعميل/للعميل/العميل"
            var customerMatch = Regex.Match(
                text,
                @"(?:اسمه|اسمها|لعميل|للعميل|العميل)\s+([\p{IsArabic}\s]+?)(?=\s+(?:ب|منتج|بمنتج|كمية|بكمية|عدد)|$)");

            if (customerMatch.Success)
            {
                result.CustomerName = customerMatch.Groups[1].Value.Trim();
            }

            if (result.Intent != "CreateInvoice")
            {
                return result;
            }

            // 3) استخراج أزواج (منتج، كمية) - يدعم أكثر من منتج في نفس الأمر
            var itemMatches = Regex.Matches(
                text,
                @"منتج\s+(?<name>[\p{IsArabic}\s]+?)\s*(?:و)?(?:بكمية|كمية|عدد)\s*(?<qty>\d+|[\p{IsArabic}]+)");

            foreach (Match m in itemMatches)
            {
                var name = m.Groups["name"].Value.Trim();
                var qty = ParseArabicNumber(m.Groups["qty"].Value.Trim());

                if (!string.IsNullOrWhiteSpace(name) && qty > 0)
                {
                    result.Items.Add(new ParsedVoiceItemDto
                    {
                        ProductNameRaw = name,
                        Quantity = qty
                    });
                }
            }

            // fallback: لو النمط المتكرر أعلاه لم يلتقط شيئاً، جرّب التقاط منتج واحد فقط
            if (result.Items.Count == 0)
            {
                var singleProductMatch = Regex.Match(
                    text, @"منتج\s+([\p{IsArabic}\s]+?)(?=\s+(?:بكمية|كمية|عدد)|$)");
                var qtyOnlyMatch = Regex.Match(text, @"(?:بكمية|كمية|عدد)\s*(\d+|[\p{IsArabic}]+)");

                if (singleProductMatch.Success)
                {
                    var qty = qtyOnlyMatch.Success ? ParseArabicNumber(qtyOnlyMatch.Groups[1].Value) : 1;
                    result.Items.Add(new ParsedVoiceItemDto
                    {
                        ProductNameRaw = singleProductMatch.Groups[1].Value.Trim(),
                        Quantity = qty > 0 ? qty : 1
                    });
                }
            }

            return result;
        }

        private static int ParseArabicNumber(string token)
        {
            token = token.Trim();
            if (int.TryParse(token, out var n)) return n;
            return ArabicNumberWords.TryGetValue(token, out var val) ? val : 0;
        }

        // =========================================================
        //                  CreateInvoice
        // =========================================================

        private async Task<VoiceCommandResultDto> HandleCreateInvoiceAsync(ParsedVoiceCommandDto parsed, CancellationToken ct)
        {
            if (parsed.Items.Count == 0)
            {
                return new VoiceCommandResultDto
                {
                    Success = false,
                    Action = "CreateInvoice",
                    Message = "لم أتمكن من التعرف على أي منتجات في الأمر الصوتي.",
                    ParsedCommand = parsed
                };
            }

            var orderItems = new List<CreateOrderItemDto>();
            var unmatched = new List<string>();

            foreach (var item in parsed.Items)
            {
                var match = await FindBestProductMatchAsync(item.ProductNameRaw, ct);
                if (match == null)
                {
                    unmatched.Add(item.ProductNameRaw);
                    continue;
                }

                item.MatchedProductId = match.Value.Id;
                item.MatchedProductName = match.Value.Name;

                orderItems.Add(new CreateOrderItemDto
                {
                    ProductId = match.Value.Id,
                    Quantity = item.Quantity,
                    UnitPrice = match.Value.Price
                });
            }

            if (unmatched.Count > 0)
            {
                return new VoiceCommandResultDto
                {
                    Success = false,
                    Action = "CreateInvoice",
                    Message = $"تعذر التعرف على المنتجات التالية: {string.Join("، ", unmatched)}. تأكد من الاسم أو أضفه للمخزون أولاً.",
                    ParsedCommand = parsed
                };
            }

            var dto = new CreateOrderDto
            {
                CustomerName = parsed.CustomerName ?? "عميل (أمر صوتي)",
                CustomerPhone = string.Empty,
                Items = orderItems
            };

            // لو قدرنا نلاقي عميل موجود فعلاً بنفس الاسم، نربط الفاتورة به عبر إيميله
            // (OrderService.CreateAsync بيدور بالإيميل عن العميل تلقائياً).
            if (!string.IsNullOrWhiteSpace(parsed.CustomerName))
            {
                var existingCustomer = await _context.Customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.FullName != null && c.FullName.Contains(parsed.CustomerName), ct);

                if (existingCustomer != null && !string.IsNullOrEmpty(existingCustomer.Email))
                {
                    dto.CustomerEmail = existingCustomer.Email;
                }
            }

            var order = await _orderService.CreateAsync(dto);

            return new VoiceCommandResultDto
            {
                Success = true,
                Action = "CreateInvoice",
                Message = $"تم إنشاء الفاتورة رقم {order.OrderNumber} بنجاح بإجمالي {order.TotalAmount:0.##}.",
                Data = order,
                ParsedCommand = parsed
            };
        }

        // =========================================================
        //                  AddProduct
        // =========================================================

        private async Task<VoiceCommandResultDto> HandleAddProductAsync(ParsedVoiceCommandDto parsed, CancellationToken ct)
        {
            var rawText = parsed.RawText ?? string.Empty;

            var nameMatch = Regex.Match(
                rawText,
                @"منتج\s+(?:اسمه|اسمها)?\s*([\p{IsArabic}\s]+?)(?=\s+(?:سعره|سعر|بسعر)|$)");
            var priceMatch = Regex.Match(rawText, @"(?:سعره|سعر|بسعر)\s*(\d+(?:\.\d+)?)");

            if (!nameMatch.Success || string.IsNullOrWhiteSpace(nameMatch.Groups[1].Value))
            {
                return new VoiceCommandResultDto
                {
                    Success = false,
                    Action = "AddProduct",
                    Message = "لم أتمكن من التعرف على اسم المنتج المطلوب إضافته.",
                    ParsedCommand = parsed
                };
            }

            var productName = nameMatch.Groups[1].Value.Trim();
            var price = priceMatch.Success ? decimal.Parse(priceMatch.Groups[1].Value) : 0m;

            // ⚠️ تأكد أن هذه الخصائص مطابقة فعلياً لـ CreateProductDto عندك
            // (بناءً على ProductsController.cs و adminApi.js المرفقين).
            var dto = new CreateProductDto
            {
                SKU = $"VC-{DateTime.UtcNow:yyyyMMddHHmmss}",
                Name = productName,
                NameAr = productName,
                SellingPrice = price,
                QuantityPerCarton = 1,
                IsActive = true,
                TrackInventory = true,
                CategoryId = 1
            };

            try
            {
                var product = await _productService.CreateAsync(dto);

                return new VoiceCommandResultDto
                {
                    Success = true,
                    Action = "AddProduct",
                    Message = $"تم إضافة المنتج \"{productName}\" بنجاح.",
                    Data = product,
                    ParsedCommand = parsed
                };
            }
            catch (Exception ex)
            {
                return new VoiceCommandResultDto
                {
                    Success = false,
                    Action = "AddProduct",
                    Message = $"فشل إضافة المنتج: {ex.Message}",
                    ParsedCommand = parsed
                };
            }
        }

        // =========================================================
        //           Fuzzy Product Matching (local, no external API)
        // =========================================================

        private async Task<(int Id, string Name, decimal Price)?> FindBestProductMatchAsync(string rawName, CancellationToken ct)
        {
            var target = rawName.Trim();
            if (string.IsNullOrWhiteSpace(target)) return null;

            var candidates = await _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .Select(p => new { p.Id, p.Name, p.NameAr, p.SellingPrice })
                .ToListAsync(ct);

            if (candidates.Count == 0) return null;

            (int Id, string Name, decimal Price, double Score)? best = null;

            foreach (var c in candidates)
            {
                foreach (var candidateName in new[] { c.Name, c.NameAr })
                {
                    if (string.IsNullOrWhiteSpace(candidateName)) continue;

                    var score = SimilarityScore(target, candidateName);
                    if (best == null || score > best.Value.Score)
                    {
                        best = (c.Id, candidateName, c.SellingPrice, score);
                    }
                }
            }

            // عتبة تشابه دنيا 0.35 - عدّلها حسب دقة النتائج الفعلية عندك
            if (best == null || best.Value.Score < 0.35) return null;

            return (best.Value.Id, best.Value.Name, best.Value.Price);
        }

        private static double SimilarityScore(string a, string b)
        {
            a = a.Trim();
            b = b.Trim();
            if (a.Length == 0 || b.Length == 0) return 0;

            if (b.Contains(a, StringComparison.OrdinalIgnoreCase) ||
                a.Contains(b, StringComparison.OrdinalIgnoreCase))
            {
                return 0.9;
            }

            var distance = LevenshteinDistance(a, b);
            var maxLen = Math.Max(a.Length, b.Length);
            return 1.0 - (double)distance / maxLen;
        }

        private static int LevenshteinDistance(string s, string t)
        {
            var d = new int[s.Length + 1, t.Length + 1];
            for (var i = 0; i <= s.Length; i++) d[i, 0] = i;
            for (var j = 0; j <= t.Length; j++) d[0, j] = j;

            for (var i = 1; i <= s.Length; i++)
            {
                for (var j = 1; j <= t.Length; j++)
                {
                    var cost = s[i - 1] == t[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[s.Length, t.Length];
        }
    }
}