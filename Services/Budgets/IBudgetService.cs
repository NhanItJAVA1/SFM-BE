using SFM_BE.DTOs.Budgets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Services.Budgets;

public interface IBudgetService
{
    Task<List<BudgetResponseDto>> GetBudgetsAsync(long userId);

    Task<BudgetResponseDto> GetBudgetAsync(long userId, long id);

    Task<BudgetResponseDto> CreateAsync(long userId, CreateBudgetDto dto);

    Task UpdateAsync(long userId, long id, UpdateBudgetDto dto);

    Task DeleteAsync(long userId, long id);
}
