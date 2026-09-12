using System;

namespace SFM_BE.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    public long UserId { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokeAt { get; set; }

    public User User { get; set; } = null!;
}
