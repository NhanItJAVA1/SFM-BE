using System;
using System.Collections.Generic;
using SFM_BE.Enums;

namespace SFM_BE.Entities;

public class User
{
    public long Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PasswordHash { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public string Currency { get; set; } = "VND";

    public string Language { get; set; } = "vi";

    public UserStatus Status { get; set; } = UserStatus.Active;

    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    public ICollection<ExternalLogin> ExternalLogins { get; set; } = [];

    public ICollection<FinancialAccount> FinancialAccounts { get; set; } = [];

    public ICollection<Category> Categories { get; set; } = [];

    public ICollection<Transaction> Transactions { get; set; } = [];

    public ICollection<Transfer> Transfers { get; set; } = [];

    public ICollection<Budget> Budgets { get; set; } = [];

    public ICollection<Invoice> Invoices { get; set; } = [];

    public ICollection<RecurringTransaction> RecurringTransactions { get; set; } = [];
}
