﻿using Microsoft.AspNetCore.Identity;

namespace Swapify.Infrastructure.Entities;

public class RoleEntity: IdentityRole, IAuditable
{
    public string Description { get; set; } = string.Empty;

    public virtual ICollection<UserRoleEntity> UserRoles { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public string? UpdatedBy { get; set; }
}