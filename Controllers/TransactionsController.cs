using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFM_BE.DTOs.Transactions;
using SFM_BE.Services.Transactions;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFM_BE.Controllers;

[ApiController]
[Authorize]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions()
    {
        return Ok(await _transactionService.GetTransactionsAsync(GetUserId()));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetTransaction(long id)
    {
        return Ok(await _transactionService.GetTransactionAsync(GetUserId(), id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction(CreateTransactionDto dto)
    {
        var result = await _transactionService.CreateAsync(GetUserId(), dto);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateTransaction(long id, UpdateTransactionDto dto)
    {
        await _transactionService.UpdateAsync(GetUserId(), id, dto);
        return Ok();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteTransaction(long id)
    {
        await _transactionService.DeleteAsync(GetUserId(), id);
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
