using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SFM_BE.DTOs.Transactions;
using SFM_BE.Entities;
using SFM_BE.Enums;
using SFM_BE.Exceptions;
using SFM_BE.Repositories.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace SFM_BE.Services.BillScan;

public class GeminiBillScanService : IBillScanService
{
    private readonly HttpClient _httpClient;
    private readonly IGenericRepository<Category> _categoryRepo;
    private readonly string _apiKey;
    private JsonSerializerOptions? _jsonOptions;

    public GeminiBillScanService(
        HttpClient httpClient,
        IConfiguration configuration,
        IGenericRepository<Category> categoryRepo)
    {
        _httpClient = httpClient;
        _categoryRepo = categoryRepo;
        _apiKey = configuration["GEMINI_API_KEY"]
            ?? throw new InvalidOperationException("GEMINI_API_KEY is missing.");
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ScanBillResponseDto> ScanAsync(long userId, IFormFile image)
    {
        if (image.Length == 0)
            throw new ArgumentException("Image is required.");

        var categories = await _categoryRepo
            .Where(x => (x.UserId == null || x.UserId == userId) &&  x.Type == CategoryType.Expense)
            .AsNoTracking()
            .Select(x => new{x.Id, x.Name}).ToListAsync();

        var categoryList = string.Join("\n", categories.Select(x => $"- ID: {x.Id}, Name: {x.Name}"));

        using var stream = new MemoryStream();
        await image.CopyToAsync(stream);

        var base64Image = Convert.ToBase64String(stream.ToArray());
        var prompt = GeminiPrompt.BuildBillScanPrompt(categoryList);
        var request = BuildRequest(prompt, base64Image, image.ContentType);

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

        var response = await _httpClient.PostAsJsonAsync(url, request);

        response.EnsureSuccessStatusCode();

        var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>();

        var json = geminiResponse?
            .Candidates?
            .FirstOrDefault()?
            .Content?
            .Parts?
            .FirstOrDefault()?
            .Text;

        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidOperationException("Gemini returned empty response.");

        var geminiResult = JsonSerializer.Deserialize<GeminiBillResultDto>(json, _jsonOptions)
            ?? throw new BadRequestException("Cannot parse bill", "Failed to deserialize the Gemini response.");
        
        long? categoryId = geminiResult.SuggestedCategoryId;

        if (categoryId.HasValue &&
            !categories.Any(x => x.Id == categoryId.Value))
            categoryId = null;

        return new ScanBillResponseDto
        {
            CategoryId = categoryId,
            Type = TransactionType.Expense,
            Amount = geminiResult.Amount,
            TransactionDate = geminiResult.TransactionDate,
            Location = geminiResult.Location,
            Description = geminiResult.Description,
            IsExcluded = false,
            Items = geminiResult.Items
        };
    }

    private static object BuildRequest(string prompt, string base64Image, string contentType)
    {
        return new
        {
            contents = new[]
            {
            new
            {
                parts = new object[]
                {
                    new { text = prompt },
                    new
                    {
                        inline_data = new
                        {
                            mime_type = contentType,
                            data = base64Image
                        }
                    }
                }
            }
        },
            generationConfig = new
            {
                responseMimeType = "application/json"
            }
        };
    }
}