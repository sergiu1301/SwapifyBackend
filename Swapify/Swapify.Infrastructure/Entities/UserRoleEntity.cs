namespace Swapify.Infrastructure.Entities;

public class UserRoleEntity
{
    public string UserId { get; set; }

    public string RoleId { get; set; }

    public virtual UserEntity User { get; set; }

    public virtual RoleEntity Role { get; set; }
}