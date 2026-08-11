using System.Security.Claims;
using IbnAlZumar.Api.DTOs.Auth;
using IbnAlZumar.Api.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Gets profile info for the currently logged in user.</summary>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UpdateProfileRequestDto), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var profile = await _authService.GetProfileAsync(userId);
        return Ok(profile);
    }

    [HttpPost("google")]
    public async Task<ActionResult<LoginResponseDto>> GoogleLogin([FromBody] GoogleLoginRequestDto request)
    {
        var result = await _authService.GoogleLoginAsync(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    [HttpPut("update-profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        await _authService.UpdateProfileAsync(userId, request);
        return Ok(new { message = "Profile updated successfully." });
    }
}