using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Budgets;
using SFM_BE.Entities;
using SFM_BE.Exceptions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Services.Budgets;

public class BudgetService : IBudgetService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Budget> _budgetRepo;

    public BudgetService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _budgetRepo = _unitOfWork.GetRepository<Budget>();
    }

    public async Task<List<BudgetResponseDto>> GetBudgetsAsync(long userId)
    {
        var budgets = await _budgetRepo.Where(x => x.UserId == userId)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<BudgetResponseDto>>(budgets);
    }

    public async Task<BudgetResponseDto> GetBudgetAsync(long userId, long id)
    {
        var budget = await _budgetRepo.Where(x => x.UserId == userId && x.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (budget == null)
            throw new NotFoundException("Budget not found", "BUDGET_NOT_FOUND");

        return _mapper.Map<BudgetResponseDto>(budget);
    }

    public async Task CreateAsync(long userId, CreateBudgetDto dto)
    {
        var budget = _mapper.Map<Budget>(dto);
        budget.UserId = userId;
        budget.CreatedAt = System.DateTime.UtcNow;
        budget.UpdatedAt = System.DateTime.UtcNow;

        await _budgetRepo.CreateAsync(budget);
        await _unitOfWork.SaveChangesAsync();

    }

    public async Task UpdateAsync(long userId, long id, UpdateBudgetDto dto)
    {
        var budget = await _budgetRepo.Where(x => x.UserId == userId && x.Id == id)
            .FirstOrDefaultAsync();

        if (budget == null)
            throw new NotFoundException("Budget not found", "BUDGET_NOT_FOUND");

        _mapper.Map(dto, budget);
        budget.UpdatedAt = System.DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(long userId, long id)
    {
        var budget = await _budgetRepo.Where(x => x.UserId == userId && x.Id == id)
            .FirstOrDefaultAsync();

        if (budget == null)
            throw new NotFoundException("Budget not found", "BUDGET_NOT_FOUND");

        budget.DeletedAt = System.DateTime.UtcNow;

        await _budgetRepo.DeleteAsync(budget);
        await _unitOfWork.SaveChangesAsync();
    }
}
