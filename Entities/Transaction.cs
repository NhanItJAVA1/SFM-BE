using System;
using System.Collections.Generic;
using SFM_BE.Enums;

namespace SFM_BE.Entities;

public class Transaction
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long AccountId { get; set; }

    public long? CategoryId { get; set; }

    public TransactionType Type { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; }

    public string? Location { get; set; }

    public bool IsExcluded { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public User User { get; set; } = null!;

    public FinancialAccount Account { get; set; } = null!;

    public Category? Category { get; set; }

    public ICollection<TransactionAttachment> Attachments { get; set; } = [];
}
