using SFM_BE.Enums;
using System.Collections.Generic;

namespace SFM_BE.Entities;

public class Role
{
    public int Id { get; set; }

    public UserRole Name { get; set; }

    public string Description { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = [];
}
