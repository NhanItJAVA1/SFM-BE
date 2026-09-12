using SFM_BE.DTOs.Budgets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Services.BudgetAlerts;

public interface IBudgetAlertService
{
    Task<List<BudgetAlertResponseDto>> GetAlertsAsync(long userId);

    Task MarkAsReadAsync(long userId, long alertId);
}
