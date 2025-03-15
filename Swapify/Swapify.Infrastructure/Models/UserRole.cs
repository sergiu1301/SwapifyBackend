using Swapify.Contracts.Models;

namespace Swapify.Infrastructure.Models;

public class UserRole : IUserRole
{
    public string UserId { get; set; }

    public string RoleId { get; set; }
}