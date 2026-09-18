using System;
using SFM_BE.Enums;

namespace SFM_BE.DTOs.Accounts;

public class FinancialAccountResponseDto
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public AccountType Type { get; set; }

    public string Currency { get; set; } = string.Empty;

    public decimal InitialBalance { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
