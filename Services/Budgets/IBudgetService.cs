using SFM_BE.DTOs.Budgets;
using SFM_BE.Enums;

namespace SFM_BE.Services.Budgets;

public interface IBudgetService
{
    Task<List<BudgetResponseDto>> GetBudgetsAsync(long userId, DeleteType filter = DeleteType.NotDeleted);

    Task<BudgetResponseDto> GetBudgetAsync(long userId, long id);

    Task CreateAsync(long userId, CreateBudgetDto dto);

    Task UpdateAsync(long userId, long id, UpdateBudgetDto dto);

    Task DeleteSoftAsync(long userId, long id);
}
