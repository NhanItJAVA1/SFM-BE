using SFM_BE.DTOs.FinancialInsights;

namespace SFM_BE.Services.FinancialInsights
{
    public interface IFinancialInsightService
    {
        Task<FinancialInsightResponseDto> GetInsightAsync(long userId, int? month, int? year);
    }
}
