using System.Data.Common;

namespace Swapify.Contracts.Transactions;

public interface ISqlServerAtomicScope: IAtomicScope
{
    DbTransaction Transaction { get; }

    DbConnection Connection { get; }
}