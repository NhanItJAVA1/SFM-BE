using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFM_BE.Controllers;
using SFM_BE.Services.AI;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FinancialAdvisorController : BaseController
{
    private readonly IAiAdvisorService _aiService;

    public FinancialAdvisorController(IAiAdvisorService aiAdvisorService)
    {
        _aiService = aiAdvisorService;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze()
    {
        return Ok(await _aiService.GetFinancialAnalysisAsync(GetUserId()));
    }
}