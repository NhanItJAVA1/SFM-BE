using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFM_BE.DTOs.Budgets;
using SFM_BE.Services.Budgets;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
    public async Task<IActionResult> GetBudgets()
    {
        return Ok(await _budgetService.GetBudgetsAsync(GetUserId()));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetBudget(long id)
    {
        return Ok(await _budgetService.GetBudgetAsync(GetUserId(), id));
    }

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
        await _budgetService.DeleteAsync(GetUserId(), id);
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
