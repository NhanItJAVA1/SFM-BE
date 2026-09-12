using System;
using SFM_BE.Enums;

namespace SFM_BE.DTOs.Transactions;

public class UpdateTransactionDto
{
    public long AccountId { get; set; }

    public long? CategoryId { get; set; }

    public TransactionType Type { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; }

    public string? Location { get; set; }

    public bool IsExcluded { get; set; }
}
