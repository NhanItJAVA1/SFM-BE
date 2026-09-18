using SFM_BE.Enums;

namespace SFM_BE.DTOs.Categories;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;

    public CategoryType Type { get; set; }

    public string? Icon { get; set; }

    public bool IsDefault { get; set; }
}
