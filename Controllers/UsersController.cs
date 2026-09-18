using Microsoft.AspNetCore.Mvc;
using SFM_BE.DTOs.Users;
using SFM_BE.Services.User;

namespace SFM_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
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

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
    }
}
