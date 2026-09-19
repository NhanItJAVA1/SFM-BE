using Microsoft.AspNetCore.Mvc;
using SFM_BE.DTOs.Users;
using SFM_BE.Services;
using SFM_BE.Services.User;

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

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
    }
}
