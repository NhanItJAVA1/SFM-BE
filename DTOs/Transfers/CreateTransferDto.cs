using System;

namespace SFM_BE.DTOs.Transfers;

public class CreateTransferDto
{
    public long FromAccountId { get; set; }

    public long ToAccountId { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateTime TransferDate { get; set; }
}
