using SFM_BE.DTOs.Categories;
using SFM_BE.Enums;

namespace SFM_BE.Services.Categories;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetCategoriesAsync(long userId, DeleteType filter = DeleteType.NotDeleted);

    Task<CategoryResponseDto> GetCategoryAsync(long userId, long id);

    Task CreateAsync(long userId, CreateCategoryDto dto);

    Task UpdateAsync(long userId, long id, UpdateCategoryDto dto);

    Task DeleteSoftAsync(long userId, long id);
}
