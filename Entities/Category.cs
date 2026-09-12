using System;
using System.Collections.Generic;
using SFM_BE.Enums;

namespace SFM_BE.Entities;

public class Category
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public CategoryType Type { get; set; }

    public string? Icon { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public User? User { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = [];

    public ICollection<Budget> Budgets { get; set; } = [];

    public ICollection<RecurringTransaction> RecurringTransactions { get; set; } = [];
}
