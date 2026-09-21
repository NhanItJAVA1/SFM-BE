using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Accounts;
using SFM_BE.Entities;
using SFM_BE.Enums;
using SFM_BE.Exceptions;
using SFM_BE.Extensions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;

namespace SFM_BE.Services.Accounts;

public class FinancialAccountService : IFinancialAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<FinancialAccount> _accountRepo;
    private readonly IGenericRepository<Transaction> _transactionRepo;

    public FinancialAccountService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _accountRepo = _unitOfWork.GetRepository<FinancialAccount>();
        _transactionRepo = _unitOfWork.GetRepository<Transaction>();
    }

    public async Task<List<FinancialAccountResponseDto>> GetAccountsAsync(long userId, DeleteType filter = DeleteType.NotDeleted)
    {
        return await _accountRepo
            .Where(x => x.UserId == userId)
            .ExcludeDeleted()
            .AsNoTracking()
            .ProjectTo<FinancialAccountResponseDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<FinancialAccountResponseDto> GetAccountAsync(long userId, long id)
    {
        return await _accountRepo.Where(x => x.UserId == userId && x.Id == id)
           .ExcludeDeleted()
           .AsNoTracking()
           .ProjectTo<FinancialAccountResponseDto>(_mapper.ConfigurationProvider)
           .FirstOrDefaultAsync()
       ?? throw new NotFoundException("Financial account not found", "FINANCIAL_ACCOUNT_NOT_FOUND");
    }

    public async Task CreateAsync(long userId, CreateFinancialAccountDto dto)
    {
        var account = _mapper.Map<FinancialAccount>(dto, opt => opt.Items["UserId"] = userId);

        await _accountRepo.CreateAsync(account);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(long userId, long id, UpdateFinancialAccountDto dto)
    {
        var account = await _accountRepo.Where(x => x.UserId == userId && x.Id == id)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Financial account not found", "FINANCIAL_ACCOUNT_NOT_FOUND");

        _mapper.Map(dto, account);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSoftAsync(long userId, long id)
    {
        if(await _accountRepo.UpdateAsync(
            x => x.UserId == userId && x.Id == id,
            s => s.SetProperty(x => x.DeletedAt, DateTime.UtcNow)) == 0)
            throw new NotFoundException("Financial account not found", "FINANCIAL_ACCOUNT_NOT_FOUND");
    }
}
