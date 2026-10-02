using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using IbnAlZumar.API.Common.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IbnAlZumar.API.Services.Notifications.WhatsApp;

/// <summary>
/// Meta WhatsApp Cloud API client — POST /{phone-number-id}/messages, per ARCHITECTURE.md §5.2a.
/// The HttpClient's BaseAddress is set to "{WhatsApp:BaseUrl}/{WhatsApp:PhoneNumberId}/" in
/// Program.cs (see BACKEND_CHANGES_PHASE3.md §3), so this class only ever posts to "messages".
/// </summary>
public class WhatsAppCloudClient : IWhatsAppClient
{
    private readonly HttpClient _httpClient;
    private readonly WhatsAppOptions _options;
    private readonly ILogger<WhatsAppCloudClient> _logger;

    public WhatsAppCloudClient(HttpClient httpClient, IOptions<WhatsAppOptions> options, ILogger<WhatsAppCloudClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<WhatsAppSendResult> SendTemplateAsync(WhatsAppTemplateMessage message, CancellationToken ct = default)
    {
        var normalizedPhone = EgyptianPhoneNormalizer.ToE164(message.ToPhone);
        if (normalizedPhone is null)
        {
            return WhatsAppSendResult.Fail($"رقم الهاتف '{message.ToPhone}' غير صالح للإرسال عبر واتساب.");
        }

        var components = new List<object>();

        if (message.DocumentHeader is not null)
        {
            components.Add(new
            {
                type = "header",
                parameters = new object[]
                {
                    new
                    {
                        type = "document",
                        document = new { link = message.DocumentHeader.Url, filename = message.DocumentHeader.Filename }
                    }
                }
            });
        }

        if (message.BodyParameters.Count > 0)
        {
            components.Add(new
            {
                type = "body",
                parameters = message.BodyParameters.Select(p => new { type = "text", text = p }).ToArray()
            });
        }

        var payload = new
        {
            messaging_product = "whatsapp",
            to = normalizedPhone,
            type = "template",
            template = new
            {
                name = message.TemplateName,
                language = new { code = message.LanguageCode ?? _options.DefaultLanguage },
                components
            }
        };

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "messages")
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);

            using var response = await _httpClient.SendAsync(request, ct);
            var responseBody = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("WhatsApp send failed ({StatusCode}) to {Phone} template {Template}: {Body}",
                    response.StatusCode, normalizedPhone, message.TemplateName, responseBody);
                return WhatsAppSendResult.Fail($"HTTP {(int)response.StatusCode}: {responseBody}");
            }

            var parsed = JsonSerializer.Deserialize<WhatsAppApiResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var providerMessageId = parsed?.Messages?.FirstOrDefault()?.Id ?? "unknown";
            return WhatsAppSendResult.Ok(providerMessageId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "WhatsApp send threw for {Phone} template {Template}", normalizedPhone, message.TemplateName);
            return WhatsAppSendResult.Fail(ex.Message);
        }
    }

    private class WhatsAppApiResponse
    {
        [JsonPropertyName("messages")]
        public List<WhatsAppApiMessageId>? Messages { get; set; }
    }

    private class WhatsAppApiMessageId
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}
