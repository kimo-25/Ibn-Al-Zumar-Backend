namespace IbnAlZumar.API.DTOs.Ai
{
    /// <summary>
    /// الطلب القادم من الفرونت إند: نص الأمر الصوتي بعد تحويله من صوت لنص.
    /// ⚠️ يُنصح بتحويل الصوت لنص في المتصفح مباشرة عبر Web Speech API
    /// (window.SpeechRecognition) لأنها مجانية وموثوقة ولا تعتمد على أي سيرفر خارجي -
    /// بدلاً من الاعتماد على نموذج صوتي على Hugging Face مرة أخرى.
    /// </summary>
    public class VoiceCommandRequestDto
    {
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// نتيجة معالجة الأمر الصوتي.
    /// </summary>
    public class VoiceCommandResultDto
    {
        public bool Success { get; set; }

        /// <summary>CreateInvoice | AddProduct | Unknown</summary>
        public string Action { get; set; } = "Unknown";

        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// الكائن الناتج فعلياً (OrderResponseDto عند إنشاء فاتورة، أو بيانات المنتج عند الإضافة).
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// تفاصيل ما تم فهمه من النص - مفيد لعرض شاشة تأكيد للمستخدم قبل/بعد الحفظ،
        /// وأساسي لتشخيص الحالات التي يفشل فيها الفهم.
        /// </summary>
        public ParsedVoiceCommandDto? ParsedCommand { get; set; }
    }

    public class ParsedVoiceCommandDto
    {
        /// <summary>CreateInvoice | AddProduct | Unknown</summary>
        public string Intent { get; set; } = "Unknown";

        public string? CustomerName { get; set; }

        public List<ParsedVoiceItemDto> Items { get; set; } = new();

        public string? RawText { get; set; }
    }

    public class ParsedVoiceItemDto
    {
        /// <summary>اسم المنتج كما قاله المستخدم قبل المطابقة مع قاعدة البيانات</summary>
        public string ProductNameRaw { get; set; } = string.Empty;

        public int Quantity { get; set; }

        /// <summary>يُملأ بعد المطابقة الناجحة مع منتج فعلي في قاعدة البيانات</summary>
        public int? MatchedProductId { get; set; }

        public string? MatchedProductName { get; set; }
    }
}