using Microsoft.AspNetCore.Identity;

namespace Swapify.Infrastructure.Entities;

public class UserRoleEntity : IdentityUserRole<string>, IAuditable
{
    public virtual UserEntity User { get; set; }

    public virtual RoleEntity Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public string? UpdatedBy { get; set; }
}