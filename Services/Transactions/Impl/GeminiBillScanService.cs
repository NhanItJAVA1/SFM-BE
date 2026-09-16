using SFM_BE.DTOs.Transactions;
using SFM_BE.Services.Transactions;
using System.Net.Http.Json;
using System.Text.Json;

namespace SFM_BE.Services.BillScan;

public class GeminiBillScanService : IBillScanService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeminiBillScanService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["GEMINI_API_KEY"]
            ?? throw new InvalidOperationException("GEMINI_API_KEY is missing.");
    }

    public async Task<ScanBillResponseDto> ScanAsync(IFormFile image)
    {
        if (image.Length == 0)
            throw new ArgumentException("Image is required.");

        using var stream = new MemoryStream();
        await image.CopyToAsync(stream);

        var base64Image = Convert.ToBase64String(stream.ToArray());

        var request = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new
                        {
                            text = """
                                   Analyze this receipt/invoice and extract the transaction information.

                                   Return JSON only in exactly this structure:

                                   {
                                     "amount": 0,
                                     "transactionDate": "yyyy-MM-ddTHH:mm:ss",
                                     "location": "",
                                     "description": "",
                                     "items": [
                                       {
                                         "name": "",
                                         "quantity": 0,
                                         "unitPrice": 0,
                                         "amount": 0
                                       }
                                     ]
                                   }

                                   Rules:
                                   - amount is the final total amount paid.
                                   - location is the merchant/store/restaurant name and address if available.
                                   - items must contain every readable purchased item.
                                   - quantity is the purchased quantity.
                                   - unitPrice is the price of one unit.
                                   - item amount is quantity * unitPrice when determinable.
                                   - Do not include subtotal, tax, discount, total, cash, or change as items.
                                   - If a field cannot be determined, return null.
                                   - If no items can be identified, return an empty array.
                                   - Do not guess unreadable values.
                                   - Return JSON only. No markdown.

                                   IMPORTANT:
                                   - Preserve the original language exactly as written on the receipt.
                                   - Do NOT translate Vietnamese text into English.
                                   - Do NOT translate merchant names, product names, addresses, or descriptions.
                                   - If the receipt is in Vietnamese, return Vietnamese text.
                                   - Only extract information that is visible on the receipt.                                

                                   If a value cannot be determined, return null.
                                   Do not include markdown.
                                   """
                        },
                        new
                        {
                            inline_data = new
                            {
                                mime_type = image.ContentType,
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

        return JsonSerializer.Deserialize<ScanBillResponseDto>(
                   json,    
                   new JsonSerializerOptions
                   {
                       PropertyNameCaseInsensitive = true
                   })
               ?? throw new InvalidOperationException("Invalid Gemini response.");
    }
}