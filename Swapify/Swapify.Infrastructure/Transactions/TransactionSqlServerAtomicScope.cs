using System.Data;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using Swapify.Contracts.Transactions;

namespace Swapify.Infrastructure.Transactions;

public class TransactionSqlServerAtomicScope : ISqlServerAtomicScope
{
    private bool _committed;

    public TransactionSqlServerAtomicScope(
        string connectionString)
    {
        Connection = new SqlConnection(connectionString);
        Connection.Open();
        Transaction = Connection.BeginTransaction(IsolationLevel.ReadCommitted);
    }

    public DbTransaction Transaction { get; }

    public DbConnection Connection { get; }

    public async ValueTask CommitAsync()
    {
        await Transaction.CommitAsync();
        _committed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_committed == false)
        {
            await Transaction.RollbackAsync();
        }

        await Transaction.DisposeAsync();
        await Connection.DisposeAsync();
    }

    public void Dispose()
    {
        if (_committed == false)
        {
            Transaction.Rollback();
        }

        Transaction.Dispose();
        Connection.Dispose();
    }
}