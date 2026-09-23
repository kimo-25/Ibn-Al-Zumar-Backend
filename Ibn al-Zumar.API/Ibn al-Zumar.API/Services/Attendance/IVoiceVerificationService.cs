namespace IbnAlZumar.API.Services.Attendance;

/// <summary>
/// NOTE: This interface file was not part of the uploaded batch. Reconstructed here
/// from usage in AttendanceService/VoiceVerificationService — signatures are unchanged
/// from your existing interface, so this should be a no-op merge if your file matches.
/// </summary>
public interface IVoiceVerificationService
{
    Task<float[]> ExtractVoiceEmbeddingAsync(Stream audioStream, string fileName, CancellationToken cancellationToken = default);

    double CalculateCosineSimilarity(float[] vectorA, float[] vectorB);
}
