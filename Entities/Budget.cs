using System;
using System.Collections.Generic;

namespace SFM_BE.Entities;

public class Budget
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long? CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal AlertThreshold { get; set; } = 80;

    public bool IsRecurring { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public User User { get; set; } = null!;

    public Category? Category { get; set; }

    public ICollection<BudgetAlert> Alerts { get; set; } = [];
}
