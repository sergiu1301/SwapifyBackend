namespace Swapify.Contracts.Models;

public interface IUser
{
    string UserId { get; }

    string UserName { get; }

    string Email { get; }

    string? FirstName { get; }

    string? LastName { get; }

    bool IsBlocked { get; }

    bool IsOnline { get; }

    bool IsAdmin { get; }

    bool EmailConfirmed { get; }

    string? RoleName { get; }

    string? RoleDescription { get; }
}