using Swapify.Contracts.Models;
using Swapify.Infrastructure.Entities;

namespace Swapify.Infrastructure.Extensions;

public static class UserRoleExtensions
{
    public static UserRoleEntity ToEntity(this IUserRole entity)
    {
        return new UserRoleEntity()
        {
            RoleId = entity.RoleId,
            UserId = entity.UserId
        };
    }
}