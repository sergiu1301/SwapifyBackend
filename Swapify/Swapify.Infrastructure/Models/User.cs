using Swapify.Contracts.Models;

namespace Swapify.Infrastructure.Models;

public class User : IUser
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool IsBlocked { get; set; }

    public bool IsOnline { get; set; }

    public bool IsAdmin { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? RoleName { get; set; }

    public string? RoleDescription { get; set; }
}