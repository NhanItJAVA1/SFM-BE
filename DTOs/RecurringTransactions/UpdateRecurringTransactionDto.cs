using System;
using SFM_BE.Enums;

namespace SFM_BE.DTOs.RecurringTransactions;

public class UpdateRecurringTransactionDto
{
    public long AccountId { get; set; }

    public long? CategoryId { get; set; }

    public TransactionType Type { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public RecurringFrequency Frequency { get; set; }

    public DateTime NextExecutionDate { get; set; }

    public bool IsActive { get; set; }
}
