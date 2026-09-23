using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Transactions;
using SFM_BE.Entities;
using SFM_BE.Enums;
using SFM_BE.Repositories.Generic;
using SFM_BE.Services.AI;

namespace SFM_BE.Services.BillScan;

public class GeminiBillScanService : IBillScanService
{
    private readonly IGeminiService _geminiService;
    private readonly IGenericRepository<Category> _categoryRepo;

    public GeminiBillScanService(IGeminiService geminiService, IGenericRepository<Category> categoryRepo)
    {
        _geminiService = geminiService;
        _categoryRepo = categoryRepo;
    }

    public async Task<ScanBillResponseDto> ScanAsync(long userId, IFormFile image)
    {
        if (image.Length == 0)
            throw new ArgumentException("Image is required.");

        var categories = await _categoryRepo
            .Where(x => (x.UserId == null || x.UserId == userId) && x.Type == CategoryType.Expense)
            .AsNoTracking()
            .Select(x => new { x.Id, x.Name })
            .ToListAsync();

        var categoryList = string.Join("\n", categories.Select(x => $"- ID: {x.Id}, Name: {x.Name}"));

        using var stream = new MemoryStream();
        await image.CopyToAsync(stream);

        var base64Image = Convert.ToBase64String(stream.ToArray());
        var prompt = GeminiPrompt.BuildBillScanPrompt(categoryList);

        var result = await _geminiService.GenerateAsync<GeminiBillResultDto>(prompt, base64Image, image.ContentType);

        var categoryId = result.SuggestedCategoryId;

        if (categoryId.HasValue && !categories.Any(x => x.Id == categoryId.Value))
            categoryId = null;

        return new ScanBillResponseDto
        {
            CategoryId = categoryId,
            Type = TransactionType.Expense,
            Amount = result.Amount,
            TransactionDate = result.TransactionDate,
            Location = result.Location,
            Description = result.Description,
            IsExcluded = false,
            Items = result.Items
        };
    }
}