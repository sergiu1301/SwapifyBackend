using Swapify.Contracts.Models;

namespace Swapify.Infrastructure.Models;

public class UserDetail : IUserDetail
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public string Salt { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool IsBlocked { get; set; } = false;

    public bool EmailConfirmed { get; set; } = false;

    public ICollection<string> UserRoles { get; set; }
}