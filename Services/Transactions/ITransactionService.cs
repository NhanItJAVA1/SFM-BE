using SFM_BE.DTOs.Transactions;
using SFM_BE.Enums;

namespace SFM_BE.Services.Transactions;

public interface ITransactionService
{
    Task<List<TransactionResponseDto>> GetTransactionsAsync(long userId, DeleteType filter = DeleteType.NotDeleted);

    Task<TransactionResponseDto> GetTransactionAsync(long userId, long id);

    Task CreateAsync(long userId, CreateTransactionDto dto);

    Task UpdateAsync(long userId, long id, UpdateTransactionDto dto);

    Task DeleteSoftAsync(long userId, long id);
}
