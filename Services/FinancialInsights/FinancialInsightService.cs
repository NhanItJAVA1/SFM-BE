using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.FinancialInsights;
using SFM_BE.Entities;
using SFM_BE.Enums;
using SFM_BE.Extensions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Services.AI;

namespace SFM_BE.Services.FinancialInsights;

public class FinancialInsightService : IFinancialInsightService
{
    private readonly IGenericRepository<Transaction> _transactionRepo;
    private readonly IGeminiService _geminiService;

    public FinancialInsightService(
        IGenericRepository<Transaction> transactionRepo,
        IGenericRepository<Budget> budgetRepo,
        IGeminiService geminiService)
    {
        _transactionRepo = transactionRepo;
        _geminiService = geminiService;
    }

    public async Task<FinancialInsightResponseDto> GetInsightAsync(long userId, int? month,  int? year)
    {
        var now = DateTime.Now;

        var selectedMonth = month ?? now.Month;
        var selectedYear = year ?? now.Year;

        var currentStart = new DateTime(selectedYear, selectedMonth, 1);
        var currentEnd = currentStart.AddMonths(1);

        var previousStart = currentStart.AddMonths(-1);
        var previousEnd = currentStart;

        var currentTransactions = await _transactionRepo
            .Where(x => x.Account.UserId == userId && !x.IsExcluded && 
            x.TransactionDate >= currentStart && x.TransactionDate < currentEnd)
            .ExcludeDeleted()
            .AsNoTracking()
            .ToListAsync();

        var previousTransactions = await _transactionRepo
            .Where(x => x.Account.UserId == userId && !x.IsExcluded &&
                x.TransactionDate >= previousStart && x.TransactionDate < previousEnd)
            .ExcludeDeleted()
            .AsNoTracking()
            .ToListAsync();

        var data = new FinancialInsightDataDto
        {
            Month = selectedMonth,
            Year = selectedYear,

            TotalIncome = currentTransactions
                .Where(x => x.Type == TransactionType.Income)
                .Sum(x => x.Amount),

            TotalExpense = currentTransactions
                .Where(x => x.Type == TransactionType.Expense)
                .Sum(x => x.Amount),

            PreviousIncome = previousTransactions
                .Where(x => x.Type == TransactionType.Income)
                .Sum(x => x.Amount),

            PreviousExpense = previousTransactions
                .Where(x => x.Type == TransactionType.Expense)
                .Sum(x => x.Amount)
        };

        return await GenerateInsightAsync(data);
    }

    private async Task<FinancialInsightResponseDto> GenerateInsightAsync(FinancialInsightDataDto data)
    {
        var prompt = GeminiPrompt.BuildFinancialInsight(data);
        return await _geminiService.GenerateAsync<FinancialInsightResponseDto>(prompt);
    }
}