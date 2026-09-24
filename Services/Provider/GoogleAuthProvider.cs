using Google.Apis.Auth;
using SFM_BE.Enums;
using SFM_BE.Exceptions;
using SFM_BE.Services.Auth.Models;

namespace SFM_BE.Services.Provider;

public class GoogleAuthProvider : IExternalAuthProvider
{
    private readonly HttpClient _httpClient;

    public GoogleAuthProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public AuthProvider Provider => AuthProvider.Google;

    public async Task<ExternalUserInfo> ValidateAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedException("Invalid Google token", "INVALID_GOOGLE_TOKEN");
        else if (!IsJwt(token))
            throw new UnauthorizedException("Only Google ID tokens are supported", "INVALID_GOOGLE_TOKEN_TYPE");

        var externalUser = await ValidateGoogleIdTokenAsync(token);
        if (!externalUser.EmailVerified)
            throw new UnauthorizedException("Google email is not verified", "GOOGLE_EMAIL_NOT_VERIFIED");

        return externalUser;
    }

    private static async Task<ExternalUserInfo> ValidateGoogleIdTokenAsync(string token)
    {
        var clientIds = GetGoogleClientIds();

        if (clientIds.Count == 0)
            throw new InvalidOperationException("At least one Google client id is required.");

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                token,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = clientIds
                });

            return new ExternalUserInfo
            {
                ProviderUserId = payload.Subject,
                Email = payload.Email,
                DisplayName = payload.Name,
                AvatarUrl = payload.Picture,
                EmailVerified = payload.EmailVerified
            };
        }
        catch (InvalidJwtException)
        {
            throw new UnauthorizedException("Invalid Google token", "INVALID_GOOGLE_TOKEN");
        }
    }

    private static IReadOnlyList<string> GetGoogleClientIds()
    {
        string?[] clientIds =
        [
            Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID"),
            Environment.GetEnvironmentVariable("GOOGLE_ANDROID_CLIENT_ID"),
            Environment.GetEnvironmentVariable("GOOGLE_IOS_CLIENT_ID")
        ];

        return clientIds
            .Where(clientId => !string.IsNullOrWhiteSpace(clientId))
            .Select(clientId => clientId!.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    private static bool IsJwt(string token) => System.Linq.Enumerable.Count(token, x => x == '.') == 2;
        
}
