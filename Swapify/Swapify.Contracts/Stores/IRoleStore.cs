using Swapify.Contracts.Filters;
using Swapify.Contracts.Models;
using Swapify.Contracts.Transactions;

namespace Swapify.Contracts.Stores;

public interface IRoleStore
{
    Task DeleteAsync(IRoleFilter filter, IAtomicScope atomicScope);
    Task<IReadOnlyList<IRole>> GetManyAsync(IRoleFilter filter, IAtomicScope atomicScope);
    Task UpdateAsync(IRole role, IAtomicScope atomicScope);
    Task<IRole> CreateAsync(IRole role, IAtomicScope atomicScope);
    Task<bool> ExistsAsync(IRoleFilter filter, IAtomicScope atomicScope);
}