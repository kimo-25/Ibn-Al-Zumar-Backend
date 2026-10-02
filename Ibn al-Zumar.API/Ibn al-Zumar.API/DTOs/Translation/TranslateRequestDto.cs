using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Translation
{
    public class TranslateRequestDto
    {
        [Required]
        public string Text { get; set; } = null!;

        /// <summary>
        /// Optional. Use "auto" to let the provider detect the source language.
        /// e.g. "en" or "ar"
        /// </summary>
        public string? SourceLanguage { get; set; }

        /// <summary>
        /// Required. Target language code, e.g. "en" or "ar"
        /// </summary>
        [Required]
        public string TargetLanguage { get; set; } = null!;
    }
}