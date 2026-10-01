using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFM_BE.Services.AI;
using System.Security.Claims;

[Authorize] // Đảm bảo đã đăng nhập JWT
[ApiController]
[Route("api/[controller]")]
public class FinancialAdvisorController : ControllerBase
{
    private readonly IAiAdvisorService _aiService;

    public FinancialAdvisorController(IAiAdvisorService aiAdvisorService)
    {
        _aiService = aiAdvisorService;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze()
    {
        // 1. Tự động trích xuất UserId từ Token JWT của người đang đăng nhập
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
        {
            return Unauthorized("Không tìm thấy thông tin định danh của người dùng từ Token.");
        }

        // 2. Gọi service xử lý với đúng ID của họ
        var analysisResult = await _aiService.GetFinancialAnalysisAsync(userId);

        if (analysisResult == null)
            return StatusCode(500, "Không thể kết nối hoặc xử lý dữ liệu từ AI Service.");

        return Ok(analysisResult);
    }
}