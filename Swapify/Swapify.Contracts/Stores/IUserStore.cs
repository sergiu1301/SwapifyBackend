using Swapify.Contracts.Filters;
using Swapify.Contracts.Models;
using Swapify.Contracts.Transactions;

namespace Swapify.Contracts.Stores;

public interface IUserStore
{
    Task DeleteAsync(IUserFilter filter, IAtomicScope atomicScope);
    Task<IUser> GetAsync(IUserFilter filter, IAtomicScope atomicScope);
    Task ConfirmEmailAsync(IUserFilter filter, string token, IAtomicScope atomicScope);
    Task<IUser> CheckPasswordAsync(IUserFilter filter, string userPassword, IAtomicScope atomicScope);
    Task ResetPasswordAsync(IUserFilter filter, string token, string newPassword, IAtomicScope atomicScope);
    Task<(int, IReadOnlyList<IUser>)> GetManyAsync(int pageNumber, int pageSize, IAtomicScope atomicScope, string? query = null);
    Task<IUser> BlockUserAsync(IUserFilter filter, IAtomicScope atomicScope);
    Task<IUser> UnblockUserAsync(IUserFilter filter, IAtomicScope atomicScope);
}