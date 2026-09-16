namespace SFM_BE.DTOs.Transactions
{
    public class CreateTransactionItemDto
    {        public string Name { get; set; } = string.Empty;
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Amount { get; set; }
    }
}
