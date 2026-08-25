using IbnAlZumar.API.DTOs.Ai;

namespace IbnAlZumar.API.Services.Ai
{
    public interface IVoiceCommandService
    {
        /// <summary>
        /// يحلل نص الأمر الصوتي (بعد تحويله من صوت لنص في الفرونت إند) ويحوّله
        /// إلى عملية حقيقية (إنشاء فاتورة / إضافة منتج) محفوظة في قاعدة البيانات.
        /// </summary>
        Task<VoiceCommandResultDto> ProcessCommandAsync(string text, string? actingUserEmail, CancellationToken cancellationToken = default);
    }
}