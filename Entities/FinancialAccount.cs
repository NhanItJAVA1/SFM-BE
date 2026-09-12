using System;
using System.Collections.Generic;
using SFM_BE.Enums;

namespace SFM_BE.Entities;

public class FinancialAccount
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public AccountType Type { get; set; }

    public string Currency { get; set; } = "VND";

    public decimal InitialBalance { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public User User { get; set; } = null!;

    public ICollection<Transaction> Transactions { get; set; } = [];

    public ICollection<Transfer> FromTransfers { get; set; } = [];

    public ICollection<Transfer> ToTransfers { get; set; } = [];

    public ICollection<RecurringTransaction> RecurringTransactions { get; set; } = [];
}
