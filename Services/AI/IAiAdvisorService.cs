using System.Threading.Tasks;
using SFM_BE.DTOs.Transactions;

namespace SFM_BE.Services.AI
{
    public interface IAiAdvisorService
    {
        Task<AiAnalyzeResponseDto?> GetFinancialAnalysisAsync(long userId, int periodDays = 90);
    }
}