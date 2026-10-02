namespace IbnAlZumar.API.DTOs.Translation
{
    public class TranslateResponseDto
    {
        public string? TranslatedText { get; set; }

        public string? SourceLanguage { get; set; }

        public string? TargetLanguage { get; set; }

        public bool Success { get; set; }

        public string? Message { get; set; }
    }
}