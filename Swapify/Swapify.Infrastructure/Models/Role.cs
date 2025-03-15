using Swapify.Contracts.Models;

namespace Swapify.Infrastructure.Models;

public class Role : IRole
{
    public string RoleId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}