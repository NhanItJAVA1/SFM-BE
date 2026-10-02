using System.ComponentModel.DataAnnotations;

namespace SFM_BE.Entities;

public class FinancialHealthLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public long UserId { get; set; }
    public int HealthLabel { get; set; }
    public string HealthStatus { get; set; } = string.Empty;

    // Lưu dưới dạng chuỗi JSON để linh hoạt lưu trữ các chỉ số đặc trưng
    public string FeaturesJson { get; set; } = string.Empty;
    public string RecommendationsJson { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}