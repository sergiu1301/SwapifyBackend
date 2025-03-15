namespace Swapify.Infrastructure.Entities;

public class RoleEntity
{
    public string RoleId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public virtual ICollection<UserRoleEntity> UserRoles { get; set; }
}