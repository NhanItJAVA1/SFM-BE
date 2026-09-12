using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFM_BE.DTOs.Accounts;
using SFM_BE.Services.Accounts;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFM_BE.Controllers;

[ApiController]
[Authorize]
[Route("api/accounts")]
public class FinancialAccountsController : ControllerBase
{
    private readonly IFinancialAccountService _accountService;

    public FinancialAccountsController(IFinancialAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        return Ok(await _accountService.GetAccountsAsync(GetUserId()));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAccount(long id)
    {
        return Ok(await _accountService.GetAccountAsync(GetUserId(), id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(CreateFinancialAccountDto dto)
    {
        await _accountService.CreateAsync(GetUserId(), dto);
        return Ok();
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAccount(long id, UpdateFinancialAccountDto dto)
    {
        await _accountService.UpdateAsync(GetUserId(), id, dto);
        return Ok();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteAccount(long id)
    {
        await _accountService.DeleteAsync(GetUserId(), id);
        return NoContent();
    }

    private long GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("User id is missing from token.");
    }
}
