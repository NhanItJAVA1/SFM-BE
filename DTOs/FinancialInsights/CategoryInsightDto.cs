namespace SFM_BE.DTOs.FinancialInsights
{
    public class CategoryInsightDto
    {
        public long CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;

        public decimal CurrentAmount { get; set; }
        public decimal PreviousAmount { get; set; }
    }
}
