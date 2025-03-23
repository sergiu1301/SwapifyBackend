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
            RoleId = model.Id,
            Name = model.Name,
            Description = model.Description
        };
    }

    public static RoleEntity ToEntity(this IRole entity)
    {
        return new RoleEntity()
        {
            Id = entity.RoleId,
            Name = entity.Name,
            Description = entity.Description
        };
    }
}