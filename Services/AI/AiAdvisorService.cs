using System.Net.Http.Json;
using System.Text.Json;
using SFM_BE.DTOs.Transactions;

namespace SFM_BE.Services.AI;

public class AiAdvisorService : IAiAdvisorService
{
    private readonly HttpClient _httpClient;

    public AiAdvisorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // Cấu hình BaseAddress trỏ thẳng tới FastAPI của bạn
        _httpClient.BaseAddress = new Uri("http://localhost:8000/");
    }

    public async Task<AiAnalyzeResponseDto?> GetFinancialAnalysisAsync(AiAnalyzeRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("analyze", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AiAnalyzeResponseDto>();
            }
        }
        catch (Exception ex)
        {
            // Xử lý lỗi khi FastAPI chưa bật hoặc mất kết nối
            Console.WriteLine($"Lỗi kết nối tới AI Service: {ex.Message}");
        }
        return null;
    }
}