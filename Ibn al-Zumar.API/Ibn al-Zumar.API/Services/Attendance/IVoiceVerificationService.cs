namespace IbnAlZumar.API.Services.Attendance;

public interface IVoiceVerificationService
{
    /// <summary>
    /// يحلل ملف صوتي محلياً (بدون أي اتصال بالإنترنت أو API خارجي) ويستخرج منه
    /// متجه ميزات صوتية (Voice Embedding) ثابت الطول يمثّل بصمة صوت المتحدث.
    /// </summary>
    Task<float[]> ExtractVoiceEmbeddingAsync(Stream audioStream, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// يحسب نسبة التطابق بين متجهين باستخدام Cosine Similarity (القيمة بين 0 و 1).
    /// </summary>
    double CalculateCosineSimilarity(float[] vectorA, float[] vectorB);
}