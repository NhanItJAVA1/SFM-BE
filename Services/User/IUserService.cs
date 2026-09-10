using SFM_BE.DTOs.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Services.User;

public interface IUserService
{
    Task<List<UserResponseDto>> GetUsersAsync();

    Task<UserResponseDto> GetUserAsync(int id);

    Task UpdateAsync(int id, UpdateUserDto dto);

    Task DeleteAsync(int id);
}
