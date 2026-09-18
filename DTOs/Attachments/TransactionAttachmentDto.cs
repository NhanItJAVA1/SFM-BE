using System;

namespace SFM_BE.DTOs.Attachments;

public class TransactionAttachmentDto
{
    public long TransactionId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public string? FileType { get; set; }

    public DateTime CreatedAt { get; set; }
}
