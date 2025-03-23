using Swapify.Contracts.Models;
using Swapify.Infrastructure.Entities;
using Swapify.Infrastructure.Models;

namespace Swapify.Infrastructure.Extensions;

public static class UserExtensions
{
    public static IUser ToModel(this UserEntity model)
    {
        return new User()
        {
            UserId = model.Id,
            Email = model.Email!,
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.UserName,
            EmailConfirmed = model.EmailConfirmed,
            LockoutEnabled = model.LockoutEnabled,
            LockoutEnd = model.LockoutEnd,
            RoleName = model.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault()!,
            RoleDescription = model.UserRoles.Select(ur => ur.Role.Description).FirstOrDefault()!,
            UpdatedBy = model.UpdatedBy,
            UpdatedAt = model.UpdatedAt,
            CreatedBy = model.CreatedBy,
            CreatedAt = model.CreatedAt
        };
    }
}