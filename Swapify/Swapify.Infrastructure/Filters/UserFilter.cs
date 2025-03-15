using Swapify.Contracts.Filters;

namespace Swapify.Infrastructure.Filters;

public class UserFilter : IUserFilter
{
    public string? UserId { get; set; }

    public string? Email { get; set; }
}