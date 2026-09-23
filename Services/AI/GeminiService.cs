using SFM_BE.Exceptions;
using System.Text.Json;

namespace SFM_BE.Services.AI;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["GEMINI_API_KEY"]
            ?? throw new InvalidOperationException("GEMINI_API_KEY is missing.");
    }

    public Task<T> GenerateAsync<T>(string prompt)
        => GenerateAsync<T>(BuildRequest(prompt));

    public Task<T> GenerateAsync<T>(string prompt, string base64Image, string contentType)
        => GenerateAsync<T>(BuildRequest(prompt, base64Image, contentType));

    private async Task<T> GenerateAsync<T>(object request)
    {
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

        return JsonSerializer.Deserialize<T>(json, _jsonOptions)
            ?? throw new BadRequestException(
                "Cannot parse Gemini response",
                "Failed to deserialize the Gemini response.");
    }

    private static object BuildRequest(string prompt)
    {
        return new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                responseMimeType = "application/json"
            }
        };
    }

    private static object BuildRequest(
        string prompt,
        string base64Image,
        string contentType)
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