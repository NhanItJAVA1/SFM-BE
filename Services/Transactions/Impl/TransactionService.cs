using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using SFM_BE.DTOs.Transactions;
using SFM_BE.DTOs.Transactions.Statistics;
using SFM_BE.Entities;
using SFM_BE.Enums;
using SFM_BE.Exceptions;
using SFM_BE.Extensions;
using SFM_BE.Helpers.Statistics;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;
using Superpower.Model;
namespace SFM_BE.Services.Transactions.Impl;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Transaction> _transactionRepo;
    private readonly IGenericRepository<Category> _categoryRepo;
    private readonly IGenericRepository<FinancialAccount> _financialAccountRepo;
    public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _transactionRepo = _unitOfWork.GetRepository<Transaction>();
        _categoryRepo = _unitOfWork.GetRepository<Category>();
        _financialAccountRepo = _unitOfWork.GetRepository<FinancialAccount>();
    }

    public async Task<List<TransactionResponseDto>> GetTransactionsAsync(long accountId, DeleteType filter = DeleteType.NotDeleted)
    {
        var transactions = await _transactionRepo.Where(x => x.AccountId == accountId)
            .DeleteFilter(filter)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<TransactionResponseDto>>(transactions);
    }

    public async Task<TransactionResponseDto> GetTransactionAsync(long accountId, long id)
    {
        var transaction = await _transactionRepo.Where(x => x.AccountId == accountId && x.Id == id)
            .ExcludeDeleted()
            .AsNoTracking()
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Transaction not found", "TRANSACTION_NOT_FOUND");

        return _mapper.Map<TransactionResponseDto>(transaction);
    }

    public async Task CreateAsync(long userId, CreateTransactionDto dto)
    {
        var financialAccount = await _financialAccountRepo
        .Where(x => x.Id == dto.AccountId && x.UserId == userId)
        .IsActive()
        .ExcludeDeleted()
        .FirstOrDefaultAsync() ?? throw new NotFoundException("Financial account not found","FINANCIAL_ACCOUNT_NOT_FOUND");

        var transaction = _mapper.Map<Transaction>(dto, opt => opt.Items["AccountId"] = financialAccount.Id);
        //if (dto.Type == TransactionType.Expense && financialAccount.InitialBalance < dto.Amount)
        //    throw new BadRequestException("Insufficient balance", "INSUFFICIENT_BALANCE");

        if (dto.Type == TransactionType.Expense)
            financialAccount.InitialBalance -= dto.Amount;
        else
            financialAccount.InitialBalance += dto.Amount;

        await _transactionRepo.CreateAsync(transaction);
        var affected =  await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(long accountId, long id, UpdateTransactionDto dto)
    {
        var transaction = await _transactionRepo.Where(x => x.AccountId == accountId && x.Id == id)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Transaction not found", "TRANSACTION_NOT_FOUND");

        _mapper.Map(dto, transaction);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSoftAsync(long accountId, long id)
    {
        if (await _transactionRepo.UpdateAsync(
        x => x.Id == id && x.DeletedAt == null,
        s => s.SetProperty(x => x.DeletedAt, DateTime.UtcNow)) == 0)
            throw new NotFoundException("Transaction not found", "TRANSACTION_NOT_FOUND");
    }

    public async Task<CategorySpendingResponseDto> GetCategorySpendingAsync(long userId, int? month, int? year, int? compareMonth, int? compareYear)
    {
        SpendingPeriodValidator.Validate(month, year, compareMonth, compareYear);

        var now = PeriodHelper.GetVietnamNow();

        var currentYear = year ?? now.Year;
        var currentMonth = month ?? now.Month;

        var currentDate = new DateOnly(currentYear, currentMonth, 1);
        var actualDate = new DateOnly(now.Year, now.Month, 1);

        var defaultCompare = currentDate.AddMonths(-1);

        var compareDate = new DateOnly(compareYear ?? defaultCompare.Year, compareMonth ?? defaultCompare.Month, 1);

        SpendingPeriodValidator.ValidatePeriod(currentDate, compareDate, actualDate);

        var isCurrentMonth = currentDate == actualDate;

        var currentPeriod = PeriodHelper.Create(currentDate.Year, currentDate.Month, isCurrentMonth, now.Day);

        var comparePeriod = PeriodHelper.Create(compareDate.Year, compareDate.Month, isCurrentMonth, now.Day);

        var currentData = await GetCategoryAggregateAsync(userId, currentPeriod);
        var compareData = await GetCategoryAggregateAsync(userId, comparePeriod);

        return await BuildCategorySpendingResponseAsync(userId, currentPeriod, comparePeriod, currentData, compareData);
    }

    private static decimal? CalculateChange(decimal current, decimal previous)
    {
        if (previous == 0)
            return null;

        return Math.Round(
            (current - previous) / previous * 100,
            2);
    }
    private async Task<List<CategoryAggregate>> GetCategoryAggregateAsync(long userId, PeriodRange period)
    {
        return await _transactionRepo
            .Where(x => x.Account.UserId == userId && x.Type == TransactionType.Expense && !x.IsExcluded &&       
                x.TransactionDate >= period.StartUtc && x.TransactionDate < period.EndUtcExclusive)
            .ExcludeDeleted()
            .AsNoTracking()
            .GroupBy(x => x.CategoryId)
            .Select(x => new CategoryAggregate
            {
                CategoryId = x.Key,
                Amount = x.Sum(t => t.Amount),
                Count = x.Count()
            })
            .ToListAsync();
    }

    private async Task<CategorySpendingResponseDto> BuildCategorySpendingResponseAsync(long userId, PeriodRange currentPeriod, PeriodRange comparePeriod, List<CategoryAggregate> currentData, List<CategoryAggregate> compareData)
    {
        var currentMap = currentData.ToDictionary(x => x.CategoryId);
        var compareMap = compareData.ToDictionary(x => x.CategoryId);

        var categoryIds = currentMap.Keys
            .Union(compareMap.Keys)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .ToList();

        var categories = await _categoryRepo
            .Where(x => categoryIds.Contains(x.Id) &&  x.Type == CategoryType.Expense && (x.UserId == null || x.UserId == userId))
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Id);

        var totalAmount = currentData.Sum(x => x.Amount);
        var compareTotalAmount = compareData.Sum(x => x.Amount);

        var result = currentMap.Keys.Union(compareMap.Keys)
            .Select(categoryId =>
            {
                currentMap.TryGetValue(categoryId, out var current);
                compareMap.TryGetValue(categoryId, out var compare);

                var amount = current?.Amount ?? 0;
                var compareAmount = compare?.Amount ?? 0;

                Category? category = null;

                if (categoryId.HasValue)
                    categories.TryGetValue(categoryId.Value, out category);

                return new CategorySpendingDto
                {
                    CategoryId = categoryId,
                    CategoryName = category?.Name ?? "Chưa phân loại",
                    Icon = category?.Icon ?? "other",

                    Amount = amount,
                    CompareAmount = compareAmount,

                    Percentage = totalAmount == 0
                        ? 0
                        : Math.Round(amount / totalAmount * 100, 2),

                    ChangePercentage =
                        CalculateChange(amount, compareAmount),

                    TransactionCount = current?.Count ?? 0,
                    IsUncategorized = !categoryId.HasValue
                };
            })
            .Where(x => x.Amount > 0 || x.CompareAmount > 0)
            .OrderByDescending(x => x.Amount)
            .ThenBy(x => x.CategoryName)
            .ToList();

        return new CategorySpendingResponseDto
        {
            CurrentPeriod = ToPeriodDto(currentPeriod),
            ComparePeriod = ToPeriodDto(comparePeriod),

            TotalAmount = totalAmount,
            CompareTotalAmount = compareTotalAmount,

            TotalChangePercentage =
                CalculateChange(totalAmount, compareTotalAmount),

            Categories = result
        };
    }
    private static PeriodDto ToPeriodDto(PeriodRange period)
    {
        return new PeriodDto
        {
            Month = period.Month,
            Year = period.Year,
            Start = period.Start,
            End = period.End
        };
    }

}
