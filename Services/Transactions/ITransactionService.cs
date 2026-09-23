using SFM_BE.DTOs.Transactions;
using SFM_BE.DTOs.Transactions.Statistics;
using SFM_BE.Enums;

namespace SFM_BE.Services.Transactions;

public interface ITransactionService
{
    Task<List<TransactionResponseDto>> GetTransactionsAsync(long userId, long? accountId, DeleteType filter = DeleteType.NotDeleted);

    Task<TransactionResponseDto> GetTransactionAsync(long id, long? accountId, long userId);

    Task CreateAsync(long userId, CreateTransactionDto dto);

    Task UpdateAsync(long userId, long id, UpdateTransactionDto dto);

    Task DeleteSoftAsync(long userId, long id);
    Task<CategorySpendingResponseDto> GetCategorySpendingAsync(long userId, int? month, int? year, int? compareMonth, int? compareYear);
}
