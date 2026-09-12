using SFM_BE.DTOs.Accounts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Services.Accounts;

public interface IFinancialAccountService
{
    Task<List<FinancialAccountResponseDto>> GetAccountsAsync(long userId);

    Task<FinancialAccountResponseDto> GetAccountAsync(long userId, long id);

    Task<FinancialAccountResponseDto> CreateAsync(long userId, CreateFinancialAccountDto dto);

    Task UpdateAsync(long userId, long id, UpdateFinancialAccountDto dto);

    Task DeleteAsync(long userId, long id);
}
