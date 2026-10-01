using SFM_BE.DTOs.Auth;

namespace SFM_BE.Services.Auth
{
    public interface IReauthenticationService
    {
        Task VerifyAsync(long userId, ReauthenticationDto dto);
    }
}
