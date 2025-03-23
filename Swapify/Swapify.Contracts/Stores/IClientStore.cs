using Swapify.Contracts.Models;
using Swapify.Contracts.Transactions;

namespace Swapify.Contracts.Stores;

public interface IClientStore
{
    public Task<IClient> GetAsync(string clientId, IAtomicScope atomicScope);
}