using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Budgets;
using SFM_BE.DTOs.Transactions;
using SFM_BE.Entities;
using SFM_BE.Enums;
using SFM_BE.Exceptions;
using SFM_BE.Extensions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;

namespace SFM_BE.Services.Budgets;

public class BudgetService : IBudgetService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Budget> _budgetRepo;
    private readonly IGenericRepository<Category> _categoryRepo;
    private readonly IGenericRepository<Transaction> _transactionRepo;

    public BudgetService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _budgetRepo = _unitOfWork.GetRepository<Budget>();
        _categoryRepo = _unitOfWork.GetRepository<Category>();
        _transactionRepo = _unitOfWork.GetRepository<Transaction>();
    }

    public async Task<List<BudgetResponseDto>> GetBudgetsAsync(long userId, DeleteType filter = DeleteType.NotDeleted)
    {
        var budgets = await _budgetRepo.Where(x => x.UserId == userId)
            .DeleteFilter(filter)
            .Include(x => x.Category)
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<BudgetResponseDto>>(budgets);
    }

    public async Task<BudgetResponseDto> GetBudgetAsync(long userId, long id)
    {
        var budget = await _budgetRepo.Where(x => x.UserId == userId && x.Id == id)
            .Include(x => x.Category)
            .ExcludeDeleted()
            .AsNoTracking()
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Budget not found", "BUDGET_NOT_FOUND");            

        return _mapper.Map<BudgetResponseDto>(budget);
    }

    public async Task CreateAsync(long userId, CreateBudgetDto dto)
    {
        if (dto.Amount <= 0)
            throw new BadRequestException("Budget amount must be greater than 0", "INVALID_BUDGET_AMOUNT");

        if (dto.StartDate >= dto.EndDate)
            throw new BadRequestException("End date must be after start date", "INVALID_BUDGET_PERIOD");

        if (dto.AlertThreshold <= 0 || dto.AlertThreshold > 100)
            throw new BadRequestException("Alert threshold must be between 1 and 100", "INVALID_ALERT_THRESHOLD");

        var budget = _mapper.Map<Budget>(dto, opt => opt.Items["UserId"] = userId);

        await _budgetRepo.CreateAsync(budget);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(long userId, long id, UpdateBudgetDto dto)
    {
        var budget = await _budgetRepo.Where(x => x.UserId == userId && x.Id == id)
            .ExcludeDeleted()
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Budget not found", "BUDGET_NOT_FOUND");            

        _mapper.Map(dto, budget);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSoftAsync(long userId, long id)
    {
        if (await _budgetRepo.UpdateAsync(
            x => x.UserId == userId && x.Id == id,
            s => s.SetProperty(x => x.DeletedAt, DateTime.UtcNow)) == 0)
            throw new NotFoundException("Invoice not found", "INVOICE_NOT_FOUND");
    }

    public async Task<BudgetProgressDto> GetProgressAsync(long userId, long budgetId)
    {
        var budget = await _budgetRepo
            .Where(x => x.UserId == userId && x.Id == budgetId)
            .Include(x => x.Category)
            .ExcludeDeleted()
            .AsNoTracking()
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException("Budget not found", "BUDGET_NOT_FOUND");

        var query = _transactionRepo
            .Where(x => x.Account.UserId == userId && x.Type == TransactionType.Expense && !x.IsExcluded &&
                        x.TransactionDate >= budget.StartDate && x.TransactionDate <= budget.EndDate)
            .ExcludeDeleted();

        if (budget.CategoryId.HasValue)
            query = query.Where(x => x.CategoryId == budget.CategoryId);

        var spentAmount = await query.SumAsync(x => x.Amount);

        var result = _mapper.Map<BudgetProgressDto>(budget);

        result.SpentAmount = spentAmount;
        result.RemainingAmount = budget.Amount - spentAmount;
        result.UsedPercentage = budget.Amount > 0 ? Math.Round(spentAmount / budget.Amount * 100, 2) : 0;
        result.IsAlert = result.UsedPercentage >= budget.AlertThreshold;

        return result;
    }

    public async Task<BudgetProgressDetailDto> GetProgressDetailAsync(long userId, long budgetId)
    {
        var budget = await _budgetRepo
            .Where(x => x.UserId == userId && x.Id == budgetId)
            .Include(x => x.Category)
            .ExcludeDeleted()
            .AsNoTracking()
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException("Budget not found", "BUDGET_NOT_FOUND");

        var query = _transactionRepo
            .Where(x =>
                x.Account.UserId == userId &&
                x.Type == TransactionType.Expense &&
                !x.IsExcluded &&
                x.TransactionDate >= budget.StartDate &&
                x.TransactionDate <= budget.EndDate)
            .ExcludeDeleted()
            .AsNoTracking();

        if (budget.CategoryId.HasValue)
            query = query.Where(x => x.CategoryId == budget.CategoryId);

        var transactions = await query
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync();

        var spentAmount = transactions.Sum(x => x.Amount);

        var result = _mapper.Map<BudgetProgressDetailDto>(budget);

        result.SpentAmount = spentAmount;
        result.RemainingAmount = budget.Amount - spentAmount;
        result.UsedPercentage = budget.Amount > 0
            ? Math.Round(spentAmount / budget.Amount * 100, 2)
            : 0;
        result.IsAlert = result.UsedPercentage >= budget.AlertThreshold;

        result.DailySpendings = transactions
            .GroupBy(x => x.TransactionDate.Date)
            .OrderByDescending(x => x.Key)
            .Select(x => new BudgetDailySpendingDto
            {
                Date = x.Key,
                Amount = x.Sum(t => t.Amount),
                TransactionCount = x.Count(),
                UsedPercentage = budget.Amount > 0
                    ? Math.Round(x.Sum(t => t.Amount) / budget.Amount * 100, 2)
                    : 0,
                Transactions = _mapper.Map<List<TransactionResponseDto>>(x.ToList())
            })
            .ToList();

        return result;
    }
}
