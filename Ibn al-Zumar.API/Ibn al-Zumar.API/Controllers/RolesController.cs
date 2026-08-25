using IbnAlZumar.API.DTOs.Identity;
using IbnAlZumar.API.Services.Identity;
using IbnAlZumar.Persistence.Seed; // إضافة الـ namespace الخاص بـ DataSeeder
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
    [Authorize(Policy = DataSeeder.PermissionCodes.RolesManage)]
    public async Task<IActionResult> GetRoles()
    {
        var result = await _userService.GetRolesAsync();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = DataSeeder.PermissionCodes.RolesManage)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
    {
        var result = await _userService.CreateRoleAsync(dto);
        return Ok(result);
    }

    [HttpGet("permissions")]
    [Authorize(Policy = DataSeeder.PermissionCodes.PermissionsManage)]
    public async Task<IActionResult> GetPermissions()
    {
        var result = await _userService.GetAllAvailablePermissionsAsync();
        return Ok(result);
    }
}