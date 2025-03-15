using Swapify.Contracts.Models;
using Swapify.Contracts.Services;
using Swapify.Contracts.Stores;
using Swapify.Contracts.Transactions;
using Validation;

namespace Swapify.Infrastructure.Services;

public class UserRoleService : IUserRoleService
{
    private readonly IUserRoleStore _userRoleStore;
    private readonly IAtomicScopeFactory _atomicScopeFactory;

    public UserRoleService(IUserRoleStore userRoleStore, IAtomicScopeFactory atomicScopeFactory)
    {
        _userRoleStore = userRoleStore;
        _atomicScopeFactory = atomicScopeFactory;
    }

    public async Task DeleteUserRoleAsync(string userId, string roleName)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));
        Requires.NotNullOrEmpty(roleName, nameof(roleName));

        await using IAtomicScope atomicScope = _atomicScopeFactory.Create();

        await _userRoleStore.DeleteAsync(userId, roleName, atomicScope);

        await atomicScope.CommitAsync();
    }

    public async Task AddUserRoleAsync(string userId, string roleName)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));
        Requires.NotNullOrEmpty(roleName, nameof(roleName));

        await using IAtomicScope atomicScope = _atomicScopeFactory.Create();

        await _userRoleStore.AddAsync(userId, roleName, atomicScope);

        await atomicScope.CommitAsync();
    }

    public async Task AddUserRoleAsync(string userId, string roleName, IAtomicScope atomicScope)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));
        Requires.NotNullOrEmpty(roleName, nameof(roleName));

        await _userRoleStore.AddAsync(userId, roleName, atomicScope);
    }

    public async Task<IRole> GetUserRoleAsync(string userId)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));

        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        IRole userRole = await _userRoleStore.GetAsync(userId, atomicScope);

        return userRole;
    }
}