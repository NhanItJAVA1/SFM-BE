namespace SFM_BE.DTOs.FinancialInsights
{
    public class FinancialInsightResponseDto
    {
        public string Summary { get; set; } = string.Empty;
        public List<FinancialInsightItemDto> Insights { get; set; } = [];
    }
}
