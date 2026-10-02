using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Categories;
using SFM_BE.Entities;
using SFM_BE.Enums;
using SFM_BE.Exceptions;
using SFM_BE.Extensions;
using SFM_BE.Repositories;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;

namespace SFM_BE.Services.Admin;

public class AdminService : IAdminService
{
    private readonly IGenericRepository<Category> _categoryRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AdminService(IGenericRepository<Category> categoryRepo, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _categoryRepo = categoryRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<CategoryResponseDto>> GetDefaultCategoriesAsync()
    {
        var categories = await _categoryRepo
            .Where(x => x.IsDefault && x.UserId == null)
            .DeleteFilter(DeleteType.NotDeleted)
            .AsNoTracking()
            .OrderBy(x => x.Type)
            .ThenBy(x => x.Name)
            .ToListAsync();

        return _mapper.Map<List<CategoryResponseDto>>(categories);
    }

    public async Task<CategoryResponseDto> CreateDefaultCategoryAsync(CreateCategoryDto dto)
    {
        var exists = await _categoryRepo
            .Where(x => x.IsDefault && x.UserId == null && x.Name == dto.Name && x.Type == dto.Type)
            .AnyAsync();

        if (exists)
            throw new ConflictException("Default category already exists", "CATEGORY_ALREADY_EXISTS");

        var category = _mapper.Map<Category>(dto);

        category.UserId = null;
        category.IsDefault = true;
        category.CreatedAt = DateTime.UtcNow;

        await _categoryRepo.CreateAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<CategoryResponseDto> UpdateDefaultCategoryAsync(long id, UpdateCategoryDto dto)
    {
        var category = await _categoryRepo
            .Where(x => x.Id == id && x.IsDefault && x.UserId == null)
            .DeleteFilter(DeleteType.NotDeleted)
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException("Default category not found", "CATEGORY_NOT_FOUND");

        var exists = await _categoryRepo
            .Where(x => x.Id != id && x.IsDefault && x.UserId == null && x.Name == dto.Name && x.Type == dto.Type)
            .DeleteFilter(DeleteType.NotDeleted)
            .AnyAsync();

        if (exists)
            throw new ConflictException("Default category already exists", "CATEGORY_ALREADY_EXISTS");

        _mapper.Map(dto, category);
        category.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task DeleteDefaultCategoryAsync(long id)
    {
        var category = await _categoryRepo
            .Where(x => x.Id == id && x.IsDefault && x.UserId == null)
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException("Default category not found", "CATEGORY_NOT_FOUND");

        category.DeletedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
    }
}