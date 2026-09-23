namespace SFM_BE.DTOs.FinancialInsights
{
    public class BudgetInsightDto
    {
        public long BudgetId { get; set; }
        public string Name { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal UsedPercentage { get; set; }
    }
}
