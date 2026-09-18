using System;
using SFM_BE.Enums;

namespace SFM_BE.DTOs.RecurringTransactions;

public class RecurringTransactionResponseDto
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

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
