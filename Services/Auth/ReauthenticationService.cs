using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Auth;
using SFM_BE.Entities;
using SFM_BE.Enums;
using SFM_BE.Exceptions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Services.Provider;

namespace SFM_BE.Services.Auth;

public class ReauthenticationService : IReauthenticationService
{
    private readonly IGenericRepository<Entities.User> _userRepo;
    private readonly IGenericRepository<ExternalLogin> _externalLoginRepo;
    private readonly IExternalAuthProvider _googleAuthProvider;

    public ReauthenticationService(IGenericRepository<Entities.User> userRepo, IGenericRepository<ExternalLogin> externalLoginRepo, IExternalAuthProvider googleAuthProvider)
    {
        _userRepo = userRepo;
        _externalLoginRepo = externalLoginRepo;
        _googleAuthProvider = googleAuthProvider;
    }

    public async Task VerifyAsync(long userId, ReauthenticationDto dto)
    {
        var user = await _userRepo.Where(x => x.Id == userId).AsNoTracking().FirstOrDefaultAsync()
            ?? throw new NotFoundException("User not found", "USER_NOT_FOUND");

        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            if (string.IsNullOrWhiteSpace(dto.Password) || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedException("Invalid password", "INVALID_PASSWORD");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(dto.IdToken))
                throw new BadRequestException("Google authentication is required", "GOOGLE_AUTH_REQUIRED");

            var googleUser = await _googleAuthProvider.ValidateAsync(dto.IdToken);

            if (!string.Equals(googleUser.Email, user.Email, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedException("Google account does not match", "GOOGLE_ACCOUNT_MISMATCH");
        }
    }
}