using SFM_BE.DTOs.Accounts;
using SFM_BE.Enums;

namespace SFM_BE.Services.Accounts;

public interface IFinancialAccountService
{
    Task<List<FinancialAccountResponseDto>> GetAccountsAsync(long userId, DeleteType filter = DeleteType.NotDeleted);

    Task<FinancialAccountResponseDto> GetAccountAsync(long userId, long id);

    Task CreateAsync(long userId, CreateFinancialAccountDto dto);

    Task UpdateAsync(long userId, long id, UpdateFinancialAccountDto dto);

    Task DeleteSoftAsync(long userId, long id);
}
