using System;
using SFM_BE.Enums;

namespace SFM_BE.Entities;

public class Invoice
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public User User { get; set; } = null!;
}
