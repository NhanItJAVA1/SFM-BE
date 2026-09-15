using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Categories;
using SFM_BE.Entities;
using SFM_BE.Exceptions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;

namespace SFM_BE.Services.Categories;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Category> _categoryRepo;
    private readonly IGenericRepository<Budget> _budgetRepo;
    private readonly IGenericRepository<RecurringTransaction> _recurringTransactionRepo;
    private readonly IGenericRepository<Transaction> _transactionRepo;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _categoryRepo = _unitOfWork.GetRepository<Category>();
        _budgetRepo = _unitOfWork.GetRepository<Budget>();
        _recurringTransactionRepo = _unitOfWork.GetRepository<RecurringTransaction>();
        _transactionRepo = _unitOfWork.GetRepository<Transaction>();
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
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Category not found", "CATEGORY_NOT_FOUND");

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task CreateAsync(long userId, CreateCategoryDto dto)
    {
        var category = _mapper.Map<Category>(dto, opt => opt.Items["UserId"] = userId);

        await _categoryRepo.CreateAsync(category);
        await _unitOfWork.SaveChangesAsync();

    }

    public async Task UpdateAsync(long userId, long id, UpdateCategoryDto dto)
    {
        var category = await _categoryRepo.Where(x => x.UserId == userId && x.Id == id)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Category not found", "CATEGORY_NOT_FOUND");

        _mapper.Map(dto, category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(long userId, long id)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        
        if (await _categoryRepo.UpdateAsync(
            x => x.UserId == userId && x.Id == id && x.DeletedAt == null,
            s => s.SetProperty(x => x.DeletedAt, DateTime.UtcNow)) == 0)
            throw new NotFoundException("Category not found", "CATEGORY_NOT_FOUND");

        await _budgetRepo.UpdateAsync(x => x.CategoryId == id,
            s => s.SetProperty(x => x.CategoryId, (long?)null));

        await _recurringTransactionRepo.UpdateAsync(x => x.CategoryId == id,
            s => s.SetProperty(x => x.CategoryId, (long?)null));

        await _transactionRepo.UpdateAsync(x => x.CategoryId == id,
            s => s.SetProperty(x => x.CategoryId, (long?)null));

        // Fail ==> CommitAsync() can't run ==> using var transaction call DisposeAsync() ==> clean resource and rollback transaction
        await transaction.CommitAsync();
    }
}
