using System;

namespace SFM_BE.Responses;

public class ErrorResponse
{
    public bool Success { get; set; }

    public int StatusCode { get; set; }

    public string ErrorCode { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    public string Path { get; set; } = string.Empty;
}
