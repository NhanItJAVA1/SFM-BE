using SFM_BE.Enums;

namespace SFM_BE.DTOs.Auth;

public class ExternalLoginDto
{
    public AuthProvider Provider { get; set; }

    public string Token { get; set; } = string.Empty;
}
