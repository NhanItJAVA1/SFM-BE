using SFM_BE.DTOs.RecurringTransactions;
using SFM_BE.Enums;

namespace SFM_BE.Services.RecurringTransactions;

public interface IRecurringTransactionService
{
    Task<List<RecurringTransactionResponseDto>> GetRecurringTransactionsAsync(long userId, DeleteType filter = DeleteType.NotDeleted);

    Task<RecurringTransactionResponseDto> GetRecurringTransactionAsync(long userId, long id);

    Task CreateAsync(long userId, CreateRecurringTransactionDto dto);

    Task UpdateAsync(long userId, long id, UpdateRecurringTransactionDto dto);

    Task DeleteSoftAsync(long userId, long id);
}
