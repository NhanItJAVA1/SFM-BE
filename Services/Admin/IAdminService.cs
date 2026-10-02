using SFM_BE.DTOs.Categories;

namespace SFM_BE.Services.Admin
{
    public interface IAdminService
    {
        Task<List<CategoryResponseDto>> GetDefaultCategoriesAsync();
        Task<CategoryResponseDto> CreateDefaultCategoryAsync(CreateCategoryDto dto);
        Task<CategoryResponseDto> UpdateDefaultCategoryAsync(long id, UpdateCategoryDto dto);
        Task DeleteDefaultCategoryAsync(long id);
    }
}
