namespace SFM_BE.DTOs.Transactions.Statistics
{
    public class CategorySpendingDto
    {
        public long? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public decimal CompareAmount { get; set; }

        public decimal Percentage { get; set; }
        public decimal? ChangePercentage { get; set; }

        public int TransactionCount { get; set; }
        public bool IsUncategorized { get; set; }
    }
}
