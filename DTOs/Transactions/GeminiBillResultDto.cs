namespace SFM_BE.DTOs.Transactions
{
    public class GeminiBillResultDto
    {
        public decimal? Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public long? SuggestedCategoryId { get; set; }
        public List<ScanBillItemDto> Items { get; set; } = [];
    }
}
