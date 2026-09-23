using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFM_BE.Services.FinancialInsights;
using System.Security.Claims;

namespace SFM_BE.Controllers;

[ApiController]
[Authorize]
[Route("api/financial-insights")]
public class FinancialInsightsController : ControllerBase
{
    private readonly IFinancialInsightService _financialInsightService;

    public FinancialInsightsController(IFinancialInsightService financialInsightService)
    {
        _financialInsightService = financialInsightService;
    }

    [HttpGet]
    public async Task<IActionResult> GetInsight([FromQuery] int? month, [FromQuery] int? year)
        => Ok(await _financialInsightService.GetInsightAsync(GetUserId(), month, year));

    private long GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(value, out var userId) ? userId : throw new UnauthorizedAccessException("User id is missing from token.");
    }
}