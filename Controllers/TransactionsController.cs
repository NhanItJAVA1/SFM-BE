using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFM_BE.DTOs.Transactions;
using SFM_BE.Enums;
using SFM_BE.Services.BillScan;
using SFM_BE.Services.Transactions;
using System.Security.Claims;

namespace SFM_BE.Controllers;

[ApiController]
[Authorize]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IBillScanService _billScanService;

    public TransactionsController(ITransactionService transactionService, IBillScanService billScanService)
    {
        _transactionService = transactionService;
        _billScanService = billScanService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions([FromQuery] long accountId, [FromQuery] DeleteType filter = DeleteType.NotDeleted)
    {
        return Ok(await _transactionService.GetTransactionsAsync(GetUserId(), accountId, filter));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetTransaction(long id, [FromQuery] long accountId)
    {
        return Ok(await _transactionService.GetTransactionAsync(id, accountId, GetUserId()));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction(CreateTransactionDto dto)
    {
        await _transactionService.CreateAsync(GetUserId(), dto);
        return Ok();
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
        await _transactionService.DeleteSoftAsync(GetUserId(), id);
        return NoContent();
    }

    [HttpPost("scan-bill")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ScanBillAsync(IFormFile image)
    {
        var result = await _billScanService.ScanAsync(GetUserId(), image);

        return Ok(result);
    }

    [HttpGet("category-spending")]
    public async Task<IActionResult> GetCategorySpendingAsync([FromQuery] int? month, [FromQuery] int? year, [FromQuery] int? compareMonth, [FromQuery] int? compareYear)
    {
        return Ok(await _transactionService.GetCategorySpendingAsync(GetUserId(), month, year, compareMonth, compareYear));
    }

    private long GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(value, out var userId) ? userId : throw new UnauthorizedAccessException("User id is missing from token.");
    }


}
