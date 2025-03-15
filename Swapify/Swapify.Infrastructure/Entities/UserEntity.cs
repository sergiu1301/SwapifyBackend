namespace Swapify.Infrastructure.Entities;

public class UserEntity
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? Salt { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool IsBlocked { get; set; }

    public bool IsOnline { get; set; }

    public bool IsAdmin { get; set; }

    public bool EmailConfirmed { get; set; }

    public virtual ICollection<UserRoleEntity> UserRoles { get; set; }
}