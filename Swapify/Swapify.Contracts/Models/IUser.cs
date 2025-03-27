namespace Swapify.Contracts.Models;

public interface IUser
{
    string UserId { get; }

    string Email { get; }

    string? FirstName { get; }

    string? LastName { get; }

    string? UserName { get; }

    string? PhoneNumber { get; }

    bool LockoutEnabled { get; }

    DateTimeOffset? LockoutEnd { get; }

    int AccessFailedCount { get; }

    bool EmailConfirmed { get; }

    string RoleName { get; }

    string RoleDescription { get; }

    DateTime CreatedAt { get; }

    string CreatedBy { get; }

    DateTime? UpdatedAt { get; }

    string? UpdatedBy { get; }
}