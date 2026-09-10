using SFM_BE.DTOs.Auth;
using SFM_BE.DTOs.Users;

namespace SFM_BE.Services.Auth;

public interface IAuthService
{
    Task RegisterAsync(RegisterUserDto dto);

    Task<LoginResponseDto> LoginAsync(LoginUserDto dto);

    Task<LoginResponseDto> ExternalLoginAsync(ExternalLoginDto dto);

    Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);

    Task LogoutAsync(string refreshToken);
}
