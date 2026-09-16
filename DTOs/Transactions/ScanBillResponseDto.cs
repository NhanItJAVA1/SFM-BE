namespace SFM_BE.DTOs.Transactions;

public class ScanBillResponseDto
{
    public decimal? Amount { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }

    public List<ScanBillItemDto> Items { get; set; } = [];

    public decimal ItemsTotal =>
        Items.Sum(x => x.Amount ?? 0);

    public decimal? Difference =>
        Amount.HasValue ? Amount.Value - ItemsTotal : null;
}
