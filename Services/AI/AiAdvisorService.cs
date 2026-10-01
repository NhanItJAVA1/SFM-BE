using Amazon.Runtime.Internal.Util;
using Microsoft.EntityFrameworkCore;
using SFM_BE.Contexts;
using SFM_BE.DTOs.Transactions;
using SFM_BE.Entities;
using System.Text.Json;

namespace SFM_BE.Services.AI;

public class AiAdvisorService : IAiAdvisorService
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;
    private readonly ILogger<AiAdvisorService> _logger;

    public AiAdvisorService(HttpClient httpClient, AppDbContext context, ILogger<AiAdvisorService> logger)
    {
        _httpClient = httpClient;
        _context = context;
        _logger = logger;
    }

    public async Task<AiAnalyzeResponseDto?> GetFinancialAnalysisAsync(long userId, int periodDays = 90)
    {
        // Tính mốc thời gian cần lọc
        var cutoffDate = DateTime.UtcNow.AddDays(-periodDays);

        // 1. Lấy dữ liệu trực tiếp từ DB của C# lên (Đã bổ sung .Include để tránh null navigation)
        var transactions = await _context.Transactions
            .Include(t => t.Category) // <--- QUAN TRỌNG: Phải có Include để tải thông tin Category
            .Where(t => t.Account.UserId == userId && t.TransactionDate >= cutoffDate && t.DeletedAt == null)
            .ToListAsync();

        var budgets = await _context.Budgets
            .Where(b => b.UserId == userId && b.DeletedAt == null)
            .ToListAsync();

        var accounts = await _context.FinancialAccounts
            .Where(a => a.UserId == userId && a.IsActive)
            .ToListAsync();

        // 2. Đóng gói payload gửi sang Python
        var payload = new
        {
            user_id = userId,
            period_days = periodDays,
            transactions = transactions.Select(t => new
            {
                id = t.Id,
                amount = t.Amount,
                type = t.Type.ToString(),
                date = t.TransactionDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                category_id = t.CategoryId,
                category_name = t.Category?.Name ?? "Uncategorized" // Tránh null tuyệt đối
            }),
            budgets = budgets.Select(b => new
            {
                id = b.Id,
                category_id = b.CategoryId,
                category_name = b.Name,
                limit = b.Amount,
                // Tính tổng tiền chi tiêu (Expense) cho category tương ứng VÀ nằm trong khoảng thời gian của budget
                spent = transactions
                        .Where(t => t.CategoryId == b.CategoryId
                            && t.Type.ToString() == "Expense"
                            && t.TransactionDate >= b.StartDate
                            && t.TransactionDate <= b.EndDate)
                        .Sum(t => t.Amount)
            }),
            accounts = accounts.Select(a => new
            {
                id = a.Id,
                name = a.Name,
                type = a.Type.ToString(),
                balance = a.InitialBalance
            })
        };

        // Gọi log để kiểm tra
        _logger.LogInformation("Payload gửi sang Python: " + JsonSerializer.Serialize(payload));

        // 3. Gọi sang service Python FastAPI qua HTTP POST
        var response = await _httpClient.PostAsJsonAsync("http://host.docker.internal:8000/api/analyze", payload);


        if (!response.IsSuccessStatusCode)
        {
            // Đọc nội dung lỗi từ Python trả về để biết vì sao nó từ chối
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Lỗi từ Python AI Service ({response.StatusCode}): {errorContent}");
        }

        // 4. Nhận kết quả trả về map vào DTO
        var result = await response.Content.ReadFromJsonAsync<AiAnalyzeResponseDto>();

        if (result == null) return null;

        // 5. Lưu log vào DB
        var log = new FinancialHealthLog
        {
            UserId = userId,
            HealthLabel = result.HealthLabel,
            HealthStatus = result.HealthStatus,
            FeaturesJson = JsonSerializer.Serialize(result.Features),
            RecommendationsJson = JsonSerializer.Serialize(result.Recommendations),
            CreatedAt = DateTime.UtcNow
        };

        _context.FinancialHealthLogs.Add(log);
        await _context.SaveChangesAsync();

        // 6. Trả kết quả về cho Controller
        return result;
    }
}