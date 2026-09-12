using System;

namespace SFM_BE.DTOs.Invoices;

public class CreateInvoiceDto
{
    public string Title { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public string? Description { get; set; }
}
