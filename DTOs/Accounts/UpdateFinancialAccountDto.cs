using SFM_BE.Enums;

namespace SFM_BE.DTOs.Accounts;

public class UpdateFinancialAccountDto
{
    public string Name { get; set; } = string.Empty;

    public AccountType Type { get; set; }

    public string Currency { get; set; } = "VND";

    public bool IsActive { get; set; }
}
