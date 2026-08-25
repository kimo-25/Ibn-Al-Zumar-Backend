using System.Security.Claims;
using IbnAlZumar.API.DTOs.Ai;
using IbnAlZumar.API.Services.Ai;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [Authorize(Roles = "Admin,Moderator")]
    public class AiVoiceController : ControllerBase
    {
        private readonly IVoiceCommandService _voiceCommandService;

        public AiVoiceController(IVoiceCommandService voiceCommandService)
        {
            _voiceCommandService = voiceCommandService;
        }

        /// <summary>
        /// يستقبل نص الأمر الصوتي (بعد تحويله من صوت لنص عبر Web Speech API في الفرونت إند
        /// - وليس عبر أي API خارجي) ويحوّله لعملية حقيقية (فاتورة / إضافة منتج) في قاعدة البيانات.
        /// </summary>
        [HttpPost("voice-command")]
        [ProducesResponseType(typeof(VoiceCommandResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(VoiceCommandResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> HandleVoiceCommand([FromBody] VoiceCommandRequestDto dto, CancellationToken cancellationToken)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Text))
            {
                return BadRequest(new { message = "من فضلك أرسل نص الأمر الصوتي (الحقل Text)." });
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
            var result = await _voiceCommandService.ProcessCommandAsync(dto.Text, userEmail, cancellationToken);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}