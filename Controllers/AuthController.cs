using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFM_BE.DTOs.Auth;
using SFM_BE.DTOs.RefreshToken;
using SFM_BE.DTOs.Users;
using SFM_BE.Exceptions;
using SFM_BE.Services.Auth;
using System;
using System.Threading.Tasks;

namespace SFM_BE.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        await _authService.RegisterAsync(dto);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        AppendRefreshTokenCookie(result.RefreshToken);

        return Ok(new
        {
            result.AccessToken,
            result.RefreshToken,
            result.User
        });
    }

    [HttpPost("external-login")]
    public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginDto dto)
    {
        var result = await _authService.ExternalLoginAsync(dto);
        AppendRefreshTokenCookie(result.RefreshToken);

        return Ok(new
        {
            result.AccessToken,
            result.User
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto? dto)
    {
        var refreshToken = Request.Cookies["refreshToken"] ?? dto?.RefreshToken;

        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedException("Missing refresh token", "REFRESH_TOKEN_REQUIRED");

        var result = await _authService.RefreshTokenAsync(refreshToken);
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDto? dto)
    {
        var refreshToken = Request.Cookies["refreshToken"] ?? dto?.RefreshToken;

        try
        {
            if (!string.IsNullOrWhiteSpace(refreshToken))
                await _authService.LogoutAsync(refreshToken);
        }
        finally
        {
            DeleteRefreshTokenCookie();
        }

        return Ok();
    }

    private void AppendRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });
    }
}
