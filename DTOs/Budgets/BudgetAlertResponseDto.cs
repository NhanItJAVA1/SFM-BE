using System;

namespace SFM_BE.DTOs.Budgets;

public class BudgetAlertResponseDto
{
    public long Id { get; set; }

    public long BudgetId { get; set; }

    public decimal Threshold { get; set; }

    public decimal CurrentPercentage { get; set; }

    public string? Message { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
}
