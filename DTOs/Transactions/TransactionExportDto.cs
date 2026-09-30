using SFM_BE.Enums;

namespace SFM_BE.DTOs.Transactions
{
    public class TransactionExportDto
    {
        public DateTime TransactionDate { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public TransactionType Type { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
    }
}
