using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.DTOs.Identity;

namespace IbnAlZumar.API.Services.Identity;

public interface IUserManagementService
{
    // إدارة المستخدمين
    Task<PagedResultDto<UserDto>> GetUsersAsync(int pageNumber = 1, int pageSize = 10);
    Task<UserDto> GetUserByIdAsync(int id);
    Task<UserDto> CreateUserAsync(CreateUserDto dto);
    Task UpdateUserRolesAsync(int userId, UpdateUserRolesDto dto);
    Task ToggleUserStatusAsync(int userId);

    // إدارة الأدوار والصلاحيات
    Task<List<RoleDto>> GetRolesAsync();
    Task<RoleDto> CreateRoleAsync(CreateRoleDto dto);
    Task<List<string>> GetAllAvailablePermissionsAsync();
}