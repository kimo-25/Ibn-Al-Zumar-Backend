using IbnAlZumar.API.DTOs.Identity;
using IbnAlZumar.API.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Owner")]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userService;

    public UsersController(IUserManagementService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _userService.GetUsersAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        var result = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
    }

    [HttpPut("{id}/roles")]
    public async Task<IActionResult> UpdateUserRoles(int id, [FromBody] UpdateUserRolesDto dto)
    {
        await _userService.UpdateUserRolesAsync(id, dto);
        return NoContent();
    }

    [HttpPatch("{id}/toggle-status")]
    public async Task<IActionResult> ToggleUserStatus(int id)
    {
        await _userService.ToggleUserStatusAsync(id);
        return NoContent();
    }
}