using Swapify.Contracts.Models;
using Swapify.Infrastructure.Entities;

namespace Swapify.Infrastructure.Extensions;

public static class UserDetailExtensions
{
    public static UserEntity ToEntity(this IUserDetail model)
    {
        return new UserEntity()
        {
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            EmailConfirmed = model.EmailConfirmed,
            LockoutEnabled = model.IsBlocked,
            Id = model.UserId,
            UserName = model.UserName,
            PasswordHash = model.PasswordHash,
            SecurityStamp = model.Salt,
        };
    }
}