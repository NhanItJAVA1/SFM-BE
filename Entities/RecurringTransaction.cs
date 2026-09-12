using System;
using SFM_BE.Enums;

namespace SFM_BE.Entities;

public class RecurringTransaction
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long AccountId { get; set; }

    public long? CategoryId { get; set; }

    public TransactionType Type { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public RecurringFrequency Frequency { get; set; }

    public DateTime NextExecutionDate { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public User User { get; set; } = null!;

    public FinancialAccount Account { get; set; } = null!;

    public Category? Category { get; set; }
}
