using SFM_BE.Enums;

namespace SFM_BE.DTOs.Accounts;

public class CreateFinancialAccountDto
{
    public string Name { get; set; } = string.Empty;

    public AccountType Type { get; set; }

    public string Currency { get; set; } = "VND";

    public decimal InitialBalance { get; set; }
}
