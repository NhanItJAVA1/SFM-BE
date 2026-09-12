using System;

namespace SFM_BE.DTOs.Transfers;

public class TransferResponseDto
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long FromAccountId { get; set; }

    public long ToAccountId { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateTime TransferDate { get; set; }

    public DateTime CreatedAt { get; set; }
}
