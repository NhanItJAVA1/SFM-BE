using System;

namespace SFM_BE.Entities;

public class Transfer
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long FromAccountId { get; set; }

    public long ToAccountId { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateTime TransferDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;

    public FinancialAccount FromAccount { get; set; } = null!;

    public FinancialAccount ToAccount { get; set; } = null!;
}
