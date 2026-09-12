using System;

namespace SFM_BE.DTOs.Budgets;

public class BudgetResponseDto
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long? CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal AlertThreshold { get; set; }

    public bool IsRecurring { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
