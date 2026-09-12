using SFM_BE.DTOs.RecurringTransactions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Services.RecurringTransactions;

public interface IRecurringTransactionService
{
    Task<List<RecurringTransactionResponseDto>> GetRecurringTransactionsAsync(long userId);

    Task<RecurringTransactionResponseDto> GetRecurringTransactionAsync(long userId, long id);

    Task CreateAsync(long userId, CreateRecurringTransactionDto dto);

    Task UpdateAsync(long userId, long id, UpdateRecurringTransactionDto dto);

    Task DeleteAsync(long userId, long id);
}
