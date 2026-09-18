using System;
using SFM_BE.Enums;

namespace SFM_BE.DTOs.Categories;

public class CategoryResponseDto
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public CategoryType Type { get; set; }

    public string? Icon { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
