using SFM_BE.Enums;
using SFM_BE.Services.Auth.Models;
using System.Threading.Tasks;

namespace SFM_BE.Services.Provider;

public interface IExternalAuthProvider
{
    AuthProvider Provider { get; }

    Task<ExternalUserInfo> ValidateAsync(string token);
}
