using Swapify.Contracts.Models;
using Swapify.Infrastructure.Entities;
using Swapify.Infrastructure.Models;

namespace Swapify.Infrastructure.Extensions;

public static class RoleExtensions
{
    public static IRole ToModel(this RoleEntity model)
    {
        return new Role()
        {
            RoleId = model.RoleId,
            Name = model.Name,
            Description = model.Description
        };
    }

    public static RoleEntity ToEntity(this IRole entity)
    {
        return new RoleEntity()
        {
            RoleId = entity.RoleId,
            Name = entity.Name,
            Description = entity.Description
        };
    }
}