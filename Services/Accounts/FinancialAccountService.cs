using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Accounts;
using SFM_BE.Entities;
using SFM_BE.Exceptions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Services.Accounts;

public class FinancialAccountService : IFinancialAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<FinancialAccount> _accountRepo;

    public FinancialAccountService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _accountRepo = _unitOfWork.GetRepository<FinancialAccount>();
    }

    public async Task<List<FinancialAccountResponseDto>> GetAccountsAsync(long userId)
    {
        var accounts = await _accountRepo.Where(x => x.UserId == userId)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<FinancialAccountResponseDto>>(accounts);
    }

    public async Task<FinancialAccountResponseDto> GetAccountAsync(long userId, long id)
    {
        var account = await _accountRepo.Where(x => x.UserId == userId && x.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (account == null)
            throw new NotFoundException("Financial account not found", "FINANCIAL_ACCOUNT_NOT_FOUND");

        return _mapper.Map<FinancialAccountResponseDto>(account);
    }

    public async Task<FinancialAccountResponseDto> CreateAsync(long userId, CreateFinancialAccountDto dto)
    {
        var account = _mapper.Map<FinancialAccount>(dto);
        account.UserId = userId;
        account.CreatedAt = System.DateTime.UtcNow;
        account.UpdatedAt = System.DateTime.UtcNow;

        await _accountRepo.CreateAsync(account);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<FinancialAccountResponseDto>(account);
    }

    public async Task UpdateAsync(long userId, long id, UpdateFinancialAccountDto dto)
    {
        var account = await _accountRepo.Where(x => x.UserId == userId && x.Id == id)
            .FirstOrDefaultAsync();

        if (account == null)
            throw new NotFoundException("Financial account not found", "FINANCIAL_ACCOUNT_NOT_FOUND");

        _mapper.Map(dto, account);
        account.UpdatedAt = System.DateTime.UtcNow;

        await _accountRepo.UpdateAsync(account);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(long userId, long id)
    {
        var account = await _accountRepo.Where(x => x.UserId == userId && x.Id == id)
            .FirstOrDefaultAsync();

        if (account == null)
            throw new NotFoundException("Financial account not found", "FINANCIAL_ACCOUNT_NOT_FOUND");

        account.DeletedAt = System.DateTime.UtcNow;
        account.IsActive = false;

        await _accountRepo.UpdateAsync(account);
        await _unitOfWork.SaveChangesAsync();
    }
}
