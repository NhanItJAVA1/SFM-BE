using SFM_BE.DTOs.Transactions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Services.Transactions;

public interface ITransactionService
{
    Task<List<TransactionResponseDto>> GetTransactionsAsync(long userId);

    Task<TransactionResponseDto> GetTransactionAsync(long userId, long id);

    Task CreateAsync(long userId, CreateTransactionDto dto);

    Task UpdateAsync(long userId, long id, UpdateTransactionDto dto);

    Task DeleteAsync(long userId, long id);
}
