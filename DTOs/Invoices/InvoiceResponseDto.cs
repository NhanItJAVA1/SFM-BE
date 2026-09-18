using System;
using SFM_BE.Enums;

namespace SFM_BE.DTOs.Invoices;

public class InvoiceResponseDto
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public InvoiceStatus Status { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
