using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFM_BE.DTOs.Budgets;
using SFM_BE.Enums;
using SFM_BE.Services.Budgets;
using System.Security.Claims;

namespace SFM_BE.Controllers;

[ApiController]
[Authorize]
[Route("api/budgets")]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _budgetService;

    public BudgetsController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBudgets([FromQuery] DeleteType filter = DeleteType.NotDeleted)
        => Ok(await _budgetService.GetBudgetsAsync(GetUserId(), filter));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetBudget(long id)
        => Ok(await _budgetService.GetBudgetAsync(GetUserId(), id));


    [HttpPost]
    public async Task<IActionResult> CreateBudget(CreateBudgetDto dto)
    {
        await _budgetService.CreateAsync(GetUserId(), dto);
        return Ok();
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateBudget(long id, UpdateBudgetDto dto)
    {
        await _budgetService.UpdateAsync(GetUserId(), id, dto);
        return Ok();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteBudget(long id)
    {
        await _budgetService.DeleteSoftAsync(GetUserId(), id);
        return NoContent();
    }

    [HttpGet("{id:long}/progress")]
    public async Task<IActionResult> GetProgressAsync(long id)
        => Ok(await _budgetService.GetProgressAsync(GetUserId(), id));


    [HttpGet("{id:long}/progress-detail")]
    public async Task<IActionResult> GetProgressDetailAsync(long id) 
        => Ok(await _budgetService.GetProgressDetailAsync(GetUserId(), id));
    

    private long GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(value, out var userId) ? userId : throw new UnauthorizedAccessException("User id is missing from token.");
    }
}
