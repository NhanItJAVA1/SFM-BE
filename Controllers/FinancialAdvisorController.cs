using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SFM_BE.Services.AI;
using SFM_BE.Contexts;
using SFM_BE.DTOs.Transactions;

[Authorize] // Đảm bảo đã đăng nhập JWT
[ApiController]
[Route("api/[controller]")]
public class FinancialAdvisorController : ControllerBase
{
    private readonly IAiAdvisorService _aiService;
    private readonly AppDbContext _context; // DbContext của bạn

    public FinancialAdvisorController(IAiAdvisorService aiService, AppDbContext context)
    {
        _aiService = aiService;
        _context = context;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeUserFinance([FromBody] AiAnalyzeRequestDto request)
    {
        // 1. Gọi sang FastAPI lấy kết quả phân tích
        var aiResult = await _aiService.GetFinancialAnalysisAsync(request);
        if (aiResult == null)
        {
            return StatusCode(503, new { message = "Hệ thống AI tạm thời gián đoạn." });
        }

        // 2. Lưu kết quả vào Database của C# để làm lịch sử
        var log = new SFM_BE.Entities.FinancialHealthLog
        {
            UserId = request.UserId,
            HealthLabel = aiResult.HealthLabel,
            HealthStatus = aiResult.HealthStatus,
            FeaturesJson = JsonSerializer.Serialize(aiResult.Features),
            RecommendationsJson = JsonSerializer.Serialize(aiResult.Recommendations),
            CreatedAt = DateTime.UtcNow
        };

        _context.FinancialHealthLogs.Add(log);
        await _context.SaveChangesAsync();

        // 3. Trả kết quả về cho Frontend
        return Ok(aiResult);
    }
}