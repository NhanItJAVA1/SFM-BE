using Google.Apis.Auth;
using SFM_BE.Enums;
using SFM_BE.Exceptions;
using SFM_BE.Services.Auth.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        if (!IsJwt(token))
            throw new UnauthorizedException("Only Google ID tokens are supported", "INVALID_GOOGLE_TOKEN_TYPE");

        var externalUser = await ValidateGoogleIdTokenAsync(token);

        if (!externalUser.EmailVerified)
            throw new UnauthorizedException("Google email is not verified", "GOOGLE_EMAIL_NOT_VERIFIED");

        return externalUser;
    }

    private static async Task<ExternalUserInfo> ValidateGoogleIdTokenAsync(string token)
    {
        var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");

        if (string.IsNullOrWhiteSpace(clientId))
            throw new InvalidOperationException("GOOGLE_CLIENT_ID is not configured.");

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                token,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [clientId]
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

    private static bool IsJwt(string token) => System.Linq.Enumerable.Count(token, x => x == '.') == 2;
        
}
