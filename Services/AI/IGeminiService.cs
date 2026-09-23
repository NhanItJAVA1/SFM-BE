namespace SFM_BE.Services.AI
{
    public interface IGeminiService
    {
        Task<T> GenerateAsync<T>(string prompt);
        Task<T> GenerateAsync<T>(string prompt, string base64Image, string contentType);
    }
}
