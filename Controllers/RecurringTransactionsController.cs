using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFM_BE.DTOs.RecurringTransactions;
using SFM_BE.Services.RecurringTransactions;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFM_BE.Controllers;

[ApiController]
[Authorize]
[Route("api/recurring-transactions")]
public class RecurringTransactionsController : ControllerBase
{
    private readonly IRecurringTransactionService _recurringTransactionService;

    public RecurringTransactionsController(IRecurringTransactionService recurringTransactionService)
    {
        _recurringTransactionService = recurringTransactionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRecurringTransactions()
    {
        return Ok(await _recurringTransactionService.GetRecurringTransactionsAsync(GetUserId()));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetRecurringTransaction(long id)
    {
        return Ok(await _recurringTransactionService.GetRecurringTransactionAsync(GetUserId(), id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecurringTransaction(CreateRecurringTransactionDto dto)
    {
        await _recurringTransactionService.CreateAsync(GetUserId(), dto);
        return Ok();
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateRecurringTransaction(long id, UpdateRecurringTransactionDto dto)
    {
        await _recurringTransactionService.UpdateAsync(GetUserId(), id, dto);
        return Ok();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteRecurringTransaction(long id)
    {
        await _recurringTransactionService.DeleteAsync(GetUserId(), id);
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
