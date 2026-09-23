namespace SFM_BE.DTOs.FinancialInsights
{
    public class FinancialInsightDataDto
    {
        public int Month { get; set; }
        public int Year { get; set; }

        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }

        public decimal PreviousIncome { get; set; }
        public decimal PreviousExpense { get; set; }

        public List<CategoryInsightDto> Categories { get; set; } = [];
        public List<BudgetInsightDto> Budgets { get; set; } = [];
    }
}
