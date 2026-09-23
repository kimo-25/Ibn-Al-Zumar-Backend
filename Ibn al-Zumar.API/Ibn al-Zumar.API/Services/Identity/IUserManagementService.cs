using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.DTOs.Identity;

namespace IbnAlZumar.API.Services.Identity;

/// <summary>
/// NOTE: This interface file wasn't part of the uploaded batch. Reconstructed from usage
/// in UsersController/UserManagementService — existing signatures unchanged. Only
/// UpdateHourlyRateAsync is new. Merge against your real interface rather than overwriting.
/// </summary>
public interface IUserManagementService
{
    Task<PagedResultDto<UserDto>> GetUsersAsync(int pageNumber = 1, int pageSize = 10);

    Task<UserDto> GetUserByIdAsync(int id);

    Task<UserDto> CreateUserAsync(CreateUserDto dto);

    Task UpdateUserRolesAsync(int userId, UpdateUserRolesDto dto);

    Task ToggleUserStatusAsync(int userId);

    Task<List<RoleDto>> GetRolesAsync();

    Task<RoleDto> CreateRoleAsync(CreateRoleDto dto);

    Task<List<string>> GetAllAvailablePermissionsAsync();

    /// <summary>جديد: تعديل أجر الساعة لموظف (يُستخدم في PATCH /api/users/{userId}/hourly-rate).</summary>
    Task UpdateHourlyRateAsync(int userId, decimal hourlyRate);
}

