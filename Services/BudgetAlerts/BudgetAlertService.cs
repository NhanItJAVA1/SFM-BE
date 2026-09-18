using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Budgets;
using SFM_BE.Entities;
using SFM_BE.Exceptions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;

namespace SFM_BE.Services.BudgetAlerts;

public class BudgetAlertService : IBudgetAlertService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<BudgetAlert> _alertRepo;

    public BudgetAlertService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _alertRepo = _unitOfWork.GetRepository<BudgetAlert>();
    }

    public async Task<List<BudgetAlertResponseDto>> GetAlertsAsync(long userId)
    {
        var alerts = await _alertRepo.Where(x => x.Budget.UserId == userId)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<BudgetAlertResponseDto>>(alerts);
       }

    public async Task MarkAsReadAsync(long userId, long alertId)
    {
        if (await _alertRepo.UpdateAsync(
        x => x.Id == alertId && x.Budget.UserId == userId,
        s => s.SetProperty(x => x.IsRead, true)) == 0)
            throw new NotFoundException("Budget alert not found", "BUDGET_ALERT_NOT_FOUND");
    }
}
