using System.Threading;
using System.Threading.Tasks;
using IbnAlZumar.API.DTOs.Translation;
using IbnAlZumar.API.Services.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers
{
    [ApiController]
    [Route("api/v1/translation")]
    public class TranslationController : ControllerBase
    {
        private readonly ITranslationService _translationService;

        public TranslationController(ITranslationService translationService)
        {
            _translationService = translationService;
        }

        /// <summary>
        /// POST /api/v1/translation/translate
        /// Body: { text, sourceLanguage?, targetLanguage }
        /// sourceLanguage is optional; the service will use "auto" when omitted.
        /// Returns a TranslateResponseDto with translatedText (null if translation failed).
        /// </summary>
        [HttpPost("translate")]
        public async Task<IActionResult> Translate([FromBody] TranslateRequestDto request, CancellationToken ct = default)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest(new TranslateResponseDto
                {
                    Success = false,
                    Message = "Request body must include 'text'."
                });
            }

            if (string.IsNullOrWhiteSpace(request.TargetLanguage))
            {
                return BadRequest(new TranslateResponseDto
                {
                    Success = false,
                    Message = "Request body must include 'targetLanguage'."
                });
            }

            var source = string.IsNullOrWhiteSpace(request.SourceLanguage) ? "auto" : request.SourceLanguage;
            var translated = await _translationService.TranslateAsync(request.Text, source, request.TargetLanguage, ct);

            if (translated is null)
            {
                return Ok(new TranslateResponseDto
                {
                    TranslatedText = null,
                    SourceLanguage = source,
                    TargetLanguage = request.TargetLanguage,
                    Success = false,
                    Message = "Translation not available (provider error or empty result)."
                });
            }

            return Ok(new TranslateResponseDto
            {
                TranslatedText = translated,
                SourceLanguage = source,
                TargetLanguage = request.TargetLanguage,
                Success = true,
                Message = null
            });
        }
    }
}