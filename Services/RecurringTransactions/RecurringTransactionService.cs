using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.RecurringTransactions;
using SFM_BE.Entities;
using SFM_BE.Exceptions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;

namespace SFM_BE.Services.RecurringTransactions;

public class RecurringTransactionService : IRecurringTransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<RecurringTransaction> _recurringRepo;

    public RecurringTransactionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _recurringRepo = _unitOfWork.GetRepository<RecurringTransaction>();
    }

    public async Task<List<RecurringTransactionResponseDto>> GetRecurringTransactionsAsync(long accountId)
    {
        var items = await _recurringRepo.Where(x => x.AccountId == accountId)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<RecurringTransactionResponseDto>>(items);
    }

    public async Task<RecurringTransactionResponseDto> GetRecurringTransactionAsync(long accountId, long id)
    {
        var item = await _recurringRepo.Where(x => x.AccountId == accountId && x.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Recurring transaction not found", "RECURRING_TRANSACTION_NOT_FOUND");

        return _mapper.Map<RecurringTransactionResponseDto>(item);
    }

    public async Task CreateAsync(long accountId, CreateRecurringTransactionDto dto)
    {
        var item = _mapper.Map<RecurringTransaction>(dto, opt => opt.Items["AccountId"] = accountId);

        await _recurringRepo.CreateAsync(item);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(long accountId, long id, UpdateRecurringTransactionDto dto)
    {
        var item = await _recurringRepo.Where(x => x.AccountId == accountId && x.Id == id)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Recurring transaction not found", "RECURRING_TRANSACTION_NOT_FOUND");

        _mapper.Map(dto, item);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(long accountId, long id)
    {
        if (await _recurringRepo.UpdateAsync(
            x => x.AccountId == accountId && x.Id == id,
            s =>
            {
                s.SetProperty(x => x.DeletedAt, DateTime.UtcNow);
                s.SetProperty(x => x.IsActive, false);
            }) == 0)
            throw new NotFoundException("Recurring transaction not found", "RECURRING_TRANSACTION_NOT_FOUND");
    }
}
