using IbnAlZumar.API.DTOs.Identity;
using IbnAlZumar.API.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IUserManagementService _userService;

    public RolesController(IUserManagementService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var result = await _userService.GetRolesAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
    {
        var result = await _userService.CreateRoleAsync(dto);
        return Ok(result);
    }

    [HttpGet("permissions")]
    public async Task<IActionResult> GetPermissions()
    {
        var result = await _userService.GetAllAvailablePermissionsAsync();
        return Ok(result);
    }
}