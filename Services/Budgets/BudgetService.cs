using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Budgets;
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

    public BudgetService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _budgetRepo = _unitOfWork.GetRepository<Budget>();
    }

    public async Task<List<BudgetResponseDto>> GetBudgetsAsync(long userId, DeleteType filter = DeleteType.NotDeleted)
    {
        var budgets = await _budgetRepo.Where(x => x.UserId == userId)
            .DeleteFilter(filter)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<BudgetResponseDto>>(budgets);
    }

    public async Task<BudgetResponseDto> GetBudgetAsync(long userId, long id)
    {
        var budget = await _budgetRepo.Where(x => x.UserId == userId && x.Id == id)
            .ExcludeDeleted()
            .AsNoTracking()
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Budget not found", "BUDGET_NOT_FOUND");            

        return _mapper.Map<BudgetResponseDto>(budget);
    }

    public async Task CreateAsync(long userId, CreateBudgetDto dto)
    {
        var budget = _mapper.Map<Budget>(dto, opt => opt.Items["UserId"] = userId);

        await _budgetRepo.CreateAsync(budget);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(long userId, long id, UpdateBudgetDto dto)
    {
        var budget = await _budgetRepo.Where(x => x.UserId == userId && x.Id == id)
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
}
