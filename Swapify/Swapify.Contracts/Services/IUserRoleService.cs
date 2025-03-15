using Swapify.Contracts.Models;
using Swapify.Contracts.Transactions;

namespace Swapify.Contracts.Services;

public interface IUserRoleService
{
    Task DeleteUserRoleAsync(string userId, string roleName);
    Task AddUserRoleAsync(string userId, string roleName);
    Task AddUserRoleAsync(string userId, string roleName, IAtomicScope atomicScope);
    Task<IRole> GetUserRoleAsync(string userId);
}