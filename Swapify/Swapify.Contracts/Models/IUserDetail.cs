namespace Swapify.Contracts.Models;

public interface IUserDetail
{
    string UserId { get; }

    string UserName { get; }

    string Email { get; }

    string PasswordHash { get; }

    string Salt { get; }

    string? FirstName { get; }

    string? LastName { get; }

    bool IsBlocked { get; }

    bool EmailConfirmed { get; }

    ICollection<string> UserRoles { get; }
}