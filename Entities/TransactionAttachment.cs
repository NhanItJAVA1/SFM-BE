using System;

namespace SFM_BE.Entities;

public class TransactionAttachment
{
    public long Id { get; set; }

    public long TransactionId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public string? FileType { get; set; }

    public DateTime CreatedAt { get; set; }

    public Transaction Transaction { get; set; } = null!;
}
