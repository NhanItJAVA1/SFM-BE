namespace SFM_BE.Services.Auth.Models;

public class ExternalUserInfo
{
    public string ProviderUserId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? DisplayName { get; set; }

    public string? AvatarUrl { get; set; }

    public bool EmailVerified { get; set; }
}
