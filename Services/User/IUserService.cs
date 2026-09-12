using SFM_BE.DTOs.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Services.User;

public interface IUserService
{
    Task<List<UserResponseDto>> GetUsersAsync();

    Task<UserResponseDto> GetUserAsync(long id);

    Task UpdateAsync(long id, UpdateUserDto dto);

    Task DeleteAsync(long id);
}
