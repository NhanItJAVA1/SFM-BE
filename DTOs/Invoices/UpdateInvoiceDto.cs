using System;
using SFM_BE.Enums;

namespace SFM_BE.DTOs.Invoices;

public class UpdateInvoiceDto
{
    public string Title { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public InvoiceStatus Status { get; set; }

    public string? Description { get; set; }
}
