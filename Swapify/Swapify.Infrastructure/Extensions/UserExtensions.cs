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
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            EmailConfirmed = model.EmailConfirmed,
            IsBlocked = model.IsBlocked,
            IsOnline = model.IsOnline,
            IsAdmin = model.IsAdmin,
            UserId = model.UserId,
            UserName = model.UserName,
            RoleName = model.UserRoles != null ? model.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault() : null,
            RoleDescription = model.UserRoles != null ? model.UserRoles.Select(ur => ur.Role.Description).FirstOrDefault() : null
        };
    }
}