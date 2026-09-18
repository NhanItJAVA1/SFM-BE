using SFM_BE.DTOs.Transactions;

namespace SFM_BE.DTOs.Budgets
{
    public class BudgetProgressDetailDto : BudgetProgressDto
    {
        public List<BudgetDailySpendingDto> DailySpendings { get; set; } = [];
    }
    public class BudgetDailySpendingDto
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
        public decimal UsedPercentage { get; set; }
        public List<TransactionResponseDto> Transactions { get; set; } = [];
    }
}
