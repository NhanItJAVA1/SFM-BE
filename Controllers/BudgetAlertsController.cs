using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFM_BE.Services.BudgetAlerts;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFM_BE.Controllers;

[ApiController]
[Authorize]
[Route("api/budget-alerts")]
public class BudgetAlertsController : ControllerBase
{
    private readonly IBudgetAlertService _budgetAlertService;

    public BudgetAlertsController(IBudgetAlertService budgetAlertService)
    {
        _budgetAlertService = budgetAlertService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAlerts()
    {
        return Ok(await _budgetAlertService.GetAlertsAsync(GetUserId()));
    }

    [HttpPost("{id:long}/read")]
    public async Task<IActionResult> MarkAsRead(long id)
    {
        await _budgetAlertService.MarkAsReadAsync(GetUserId(), id);
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
