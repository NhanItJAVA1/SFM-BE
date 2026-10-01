using Microsoft.AspNetCore.Mvc;
using SFM_BE.DTOs.Auth;
using SFM_BE.DTOs.Users;
using SFM_BE.Services;
using SFM_BE.Services.User;
using System.Security.Claims;

namespace SFM_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly S3PresignedUrlService _s3Service;

    public UsersController(IUserService userService, S3PresignedUrlService s3Service)
    {
        _userService = userService;
        _s3Service = s3Service;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _userService.GetUsersAsync());
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetUser(long id)
    {
        return Ok(await _userService.GetUserAsync(id));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateUser(long id, UpdateUserDto dto)
    {
        await _userService.UpdateAsync(id, dto);
        return Ok();
    }

    [HttpPost("avatar/upload-url")]
    public IActionResult CreateAvatarUploadUrl(AvatarUploadRequestDto dto)
    {
        return Ok(_s3Service.CreateAvatarUploadUrl(
            dto.FileName,
            dto.ContentType));
    }

    [HttpDelete("me/data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResetAccount([FromBody] ReauthenticationDto auth)
    {
        await _userService.ResetAccountAsync(GetUserId(), auth);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
    }

    private long GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(value, out var userId) ? userId : throw new UnauthorizedAccessException("User id is missing from token.");
    }
}
