using System;

namespace SFM_BE.DTOs.Budgets;

public class CreateBudgetDto
{
    public long? CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal AlertThreshold { get; set; } = 80;

    public bool IsRecurring { get; set; }
}
