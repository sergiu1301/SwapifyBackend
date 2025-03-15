namespace Swapify.Contracts.Transactions;

public interface IAtomicScope : IAsyncDisposable, IDisposable
{
    ValueTask CommitAsync();
}