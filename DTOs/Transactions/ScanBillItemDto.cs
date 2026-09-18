namespace SFM_BE.DTOs.Transactions;

public class ScanBillItemDto
{
    public string? Name { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Amount { get; set; }
}
