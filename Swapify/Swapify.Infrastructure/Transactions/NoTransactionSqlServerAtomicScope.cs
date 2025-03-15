using Microsoft.Data.SqlClient;
using System.Data.Common;
using Swapify.Contracts.Transactions;

namespace Swapify.Infrastructure.Transactions;

public class NoTransactionSqlServerAtomicScope : ISqlServerAtomicScope
{
    public NoTransactionSqlServerAtomicScope(string connectionString)
    {
        Connection = new SqlConnection(connectionString);
        Connection.Open();
    }

    public DbTransaction Transaction
    {
        get { return null; }
    }

    public DbConnection Connection { get; }

    public ValueTask CommitAsync()
    {
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        Connection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Connection.DisposeAsync();
    }
}