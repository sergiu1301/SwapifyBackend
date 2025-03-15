using Swapify.Contracts.Filters;
using Swapify.Contracts.Models;
using Swapify.Contracts.Transactions;

namespace Swapify.Contracts.Stores;

public interface IUserStore
{
    Task DeleteAsync(string userId, IAtomicScope atomicScope);
    Task<IUser?> GetAsync(IUserFilter filter, IAtomicScope atomicScope);
    Task<IUser> GetAsync(string userEmail, string passwordHash, IAtomicScope atomicScope);
    Task ChangeUserPasswordAsync(string userId, string passwordHash, string salt, IAtomicScope atomicScope);
    Task<(int, IReadOnlyList<IUser>)> GetManyAsync(int pageNumber, int pageSize, IAtomicScope atomicScope, string? query = null);
    Task<string> GetUserSaltAsync(string userEmail, IAtomicScope atomicScope);
    Task ConfirmUserEmailAsync(string userId, IAtomicScope atomicScope);
    Task<IUser> BlockUserAsync(string userId, IAtomicScope atomicScope);
    Task<IUser> UnblockUserAsync(string userId, IAtomicScope atomicScope);
    Task<bool> ExistsAsync(string userEmail, IAtomicScope atomicScope);
    Task<IUser> CreateAsync(IUserDetail userDetail, IAtomicScope atomicScope);
    Task<IUser> ChangeUserNameAsync(string userId, string userName, string? firstName,
        string? lastName, IAtomicScope atomicScope);

    Task ActiveUserAsync(string userId, IAtomicScope atomicScope);
    Task InactiveUserAsync(string userId, IAtomicScope atomicScope);
}