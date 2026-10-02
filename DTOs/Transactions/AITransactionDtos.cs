using System.Text.Json.Serialization;

namespace SFM_BE.DTOs.Transactions
{
    public class AiAnalyzeRequestDto
    {
        public int UserId { get; set; }
        public List<AiTransactionItemDto> Transactions { get; set; } = new();
        public List<AiBudgetItemDto> Budgets { get; set; } = new();
    }
    public class AiAnalyzeResponseDto
    {
        [JsonPropertyName("user_id")]
        public long UserId { get; set; }
        [JsonPropertyName("health_label")]
        public int HealthLabel { get; set; }
        [JsonPropertyName("health_status")]
        public string HealthStatus { get; set; } = string.Empty;
        public Dictionary<string, object> Features { get; set; } = new();
        public List<AnomaliesResultDto> Anomalies { get; set; } = new();
        public List<AiRecommendationItemDto> Recommendations { get; set; } = new();
    }

    public class AnomaliesResultDto
    {
        public long TransactionId { get; set; }
        public bool IsAnomaly { get; set; }
        public double Score { get; set; }
    }
    public class AiTransactionItemDto
    {
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty; // "INCOME" hoặc "EXPENSE"
        public string Category { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    public class AiBudgetItemDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Limit { get; set; }
        public decimal Spent { get; set; }
    }

    public class AiRecommendationItemDto
    {
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
    }
}