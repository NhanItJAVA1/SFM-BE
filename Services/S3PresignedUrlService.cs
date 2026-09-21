using Amazon.S3;
using Amazon.S3.Model;

namespace SFM_BE.Services;

public sealed record PresignedUploadResult(string UploadUrl, string ObjectKey, string PublicUrl, DateTime ExpiresAtUtc);
public sealed record DirectUploadResult(string ObjectKey, string PublicUrl, DateTime UploadedAtUtc);

public class S3PresignedUrlService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _region;

    public S3PresignedUrlService(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _bucketName = configuration["AWS_BUCKET_NAME"]
            ?? throw new InvalidOperationException("AWS_BUCKET_NAME is missing.");
        _region = configuration["AWS_REGION"]
            ?? throw new InvalidOperationException("AWS_REGION is missing.");
    }

    public PresignedUploadResult CreateAvatarUploadUrl(string fileName, string contentType, int expiresMinutes = 15)
    {
        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrWhiteSpace(extension))
            extension = contentType.StartsWith("image/png", StringComparison.OrdinalIgnoreCase)
                ? ".png" : contentType.StartsWith("image/webp", StringComparison.OrdinalIgnoreCase) ? ".webp" : ".jpg";

        var objectKey = $"avatars/{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid():N}{extension}";
        return CreateUploadUrl(objectKey, contentType, expiresMinutes);
    }    

    public Task<string> CreateGetUrl(string objectKey, int expiresMinutes = 15)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = expiresAt
        };

        return Task.FromResult(_s3Client.GetPreSignedURL(request));
    }

    public async Task<string?> CreateGetUrlFromStoredUrl(string? storedUrl, int expiresMinutes = 15)
    {
        if (string.IsNullOrWhiteSpace(storedUrl))
            return null;

        var objectKey = GetObjectKeyFromUrl(storedUrl);
        return await CreateGetUrl(objectKey, expiresMinutes);
    }

    public string GetObjectKeyFromUrl(string url)
    {
        var uri = new Uri(url);
        return uri.AbsolutePath.TrimStart('/');
    }

    private PresignedUploadResult CreateUploadUrl(string objectKey, string contentType, int expiresMinutes)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiresMinutes);
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            Expires = expiresAtUtc
        };

        return new PresignedUploadResult(_s3Client.GetPreSignedURL(request), objectKey, CreatePublicUrl(objectKey), expiresAtUtc);
    }

    private string CreatePublicUrl(string objectKey) => $"https://{_bucketName}.s3.{_region}.amazonaws.com/{objectKey}";

}
