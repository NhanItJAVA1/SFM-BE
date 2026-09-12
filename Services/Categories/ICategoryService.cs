using SFM_BE.DTOs.Categories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Services.Categories;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetCategoriesAsync(long userId);

    Task<CategoryResponseDto> GetCategoryAsync(long userId, long id);

    Task<CategoryResponseDto> CreateAsync(long userId, CreateCategoryDto dto);

    Task UpdateAsync(long userId, long id, UpdateCategoryDto dto);

    Task DeleteAsync(long userId, long id);
}
