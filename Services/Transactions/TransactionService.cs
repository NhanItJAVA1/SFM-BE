using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Transactions;
using SFM_BE.Entities;
using SFM_BE.Exceptions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransactionEntity = SFM_BE.Entities.Transaction;

namespace SFM_BE.Services.Transactions;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<TransactionEntity> _transactionRepo;

    public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _transactionRepo = _unitOfWork.GetRepository<TransactionEntity>();
    }

    public async Task<List<TransactionResponseDto>> GetTransactionsAsync(long accountId)
    {
        var transactions = await _transactionRepo.Where(x => x.AccountId == accountId)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<TransactionResponseDto>>(transactions);
    }

    public async Task<TransactionResponseDto> GetTransactionAsync(long accountId, long id)
    {
        var transaction = await _transactionRepo.Where(x => x.AccountId == accountId && x.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (transaction == null)
            throw new NotFoundException("Transaction not found", "TRANSACTION_NOT_FOUND");

        return _mapper.Map<TransactionResponseDto>(transaction);
    }

    public async Task CreateAsync(long accountId, CreateTransactionDto dto)
    {
        var transaction = _mapper.Map<TransactionEntity>(dto);
        transaction.AccountId = accountId;
        transaction.CreatedAt = System.DateTime.UtcNow;
        transaction.UpdatedAt = System.DateTime.UtcNow;

        await _transactionRepo.CreateAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

    }

    public async Task UpdateAsync(long accountId    , long id, UpdateTransactionDto dto)
    {
        var transaction = await _transactionRepo.Where(x => x.AccountId == accountId && x.Id == id)
            .FirstOrDefaultAsync();

        if (transaction == null)
            throw new NotFoundException("Transaction not found", "TRANSACTION_NOT_FOUND");

        _mapper.Map(dto, transaction);
        transaction.UpdatedAt = System.DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(long accountId, long id)
    {
        var transaction = await _transactionRepo.Where(x => x.AccountId == accountId && x.Id == id)
            .FirstOrDefaultAsync();

        if (transaction == null)
            throw new NotFoundException("Transaction not found", "TRANSACTION_NOT_FOUND");

        transaction.DeletedAt = System.DateTime.UtcNow;

        await _transactionRepo.DeleteAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
    }
}
