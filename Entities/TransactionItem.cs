namespace SFM_BE.Entities
{
    public class TransactionItem
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }

        public string Name { get; set; } = string.Empty;
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Amount { get; set; }

        public Transaction Transaction { get; set; } = null!;
    }
}
