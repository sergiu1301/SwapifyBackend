using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Swapify.Contracts.Transactions;
using Validation;

namespace Swapify.Infrastructure.Extensions;

public static class AtomicScopeExtensions
{
    public static ISqlServerAtomicScope ToSqlServerAtomicScope(
        [NotNull] this IAtomicScope scope)
    {
        Requires.NotNull(scope, nameof(scope));

        if (scope is ISqlServerAtomicScope sqlServerAtomicScope)
        {
            return sqlServerAtomicScope;
        }

        throw new InvalidOperationException($"Atomic scope of type '{scope.GetType()}' can't be converted to '{typeof(ISqlServerAtomicScope)}'.");
    }

    public static async Task<TContext> ToDbContextAsync<TContext>(
        [NotNull] this IAtomicScope scope,
        [NotNull] Func<DbContextOptions<TContext>, TContext> factory)
        where TContext : DbContext
    {
        Requires.NotNull(scope, nameof(scope));
        Requires.NotNull(factory, nameof(factory));

        ISqlServerAtomicScope sqlServerAtomicScope = scope.ToSqlServerAtomicScope();
        DbContextOptionsBuilder<TContext> builder = new DbContextOptionsBuilder<TContext>();
        builder.UseSqlServer(sqlServerAtomicScope.Connection);
        TContext context = factory(builder.Options);
        await context.Database.UseTransactionAsync(sqlServerAtomicScope.Transaction);

        return context;
    }
}