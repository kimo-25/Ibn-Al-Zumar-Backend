// File: Controllers/AuthController.cs
using IbnAlZumar.Api.DTOs.Auth;
using IbnAlZumar.Api.Services.Auth;
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

    /// <summary>Authenticates a user and returns a JWT carrying role and permission claims.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        // Invalid credentials throw UnauthorizedAppException, caught by ExceptionHandlingMiddleware.
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }
}