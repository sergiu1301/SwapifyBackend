using Swapify.Contracts.Filters;

namespace Swapify.Infrastructure.Filters;

public class RoleFilter : IRoleFilter
{
    public string? RoleId { get; set; }

    public string? Name { get; set; }
}