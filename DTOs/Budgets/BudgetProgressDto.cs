namespace SFM_BE.DTOs.Budgets;

public class BudgetProgressDto
{
    public long BudgetId { get; set; }

    public long UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public long? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public decimal Amount { get; set; }

    public decimal SpentAmount { get; set; }

    public decimal RemainingAmount { get; set; }

    public decimal UsedPercentage { get; set; }

    public decimal AlertThreshold { get; set; }

    public bool IsAlert { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}