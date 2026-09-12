using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Categories;
using SFM_BE.Entities;
using SFM_BE.Exceptions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Services.Categories;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Category> _categoryRepo;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _categoryRepo = _unitOfWork.GetRepository<Category>();
    }

    public async Task<List<CategoryResponseDto>> GetCategoriesAsync(long userId)
    {
        var categories = await _categoryRepo.Where(x => x.UserId == userId || x.UserId == null)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<CategoryResponseDto>>(categories);
    }

    public async Task<CategoryResponseDto> GetCategoryAsync(long userId, long id)
    {
        var category = await _categoryRepo.Where(x => (x.UserId == userId || x.UserId == null) && x.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (category == null)
            throw new NotFoundException("Category not found", "CATEGORY_NOT_FOUND");

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task CreateAsync(long userId, CreateCategoryDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        category.UserId = userId;
        category.CreatedAt = System.DateTime.UtcNow;
        category.UpdatedAt = System.DateTime.UtcNow;

        await _categoryRepo.CreateAsync(category);
        await _unitOfWork.SaveChangesAsync();

    }

    public async Task UpdateAsync(long userId, long id, UpdateCategoryDto dto)
    {
        var category = await _categoryRepo.Where(x => x.UserId == userId && x.Id == id)
            .FirstOrDefaultAsync();

        if (category == null)
            throw new NotFoundException("Category not found", "CATEGORY_NOT_FOUND");

        _mapper.Map(dto, category);
        category.UpdatedAt = System.DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(long userId, long id)
    {
        var category = await _categoryRepo.Where(x => x.UserId == userId && x.Id == id)
            .FirstOrDefaultAsync();

        if (category == null)
            throw new NotFoundException("Category not found", "CATEGORY_NOT_FOUND");

        category.DeletedAt = System.DateTime.UtcNow;

        await _categoryRepo.DeleteAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }
}
