using Swapify.Contracts.Filters;
using Swapify.Infrastructure.Entities;
using Validation;

namespace Swapify.Infrastructure.Extensions;

public static class RoleQueryableExtensions
{
    public static IQueryable<RoleEntity> ApplyFiltering(
        this IQueryable<RoleEntity> query,
        IRoleFilter filter)
    {
        Requires.NotNull(query, nameof(query));
        Requires.NotNull(filter, nameof(filter));

        if (filter.RoleId is not null)
        {
            query = query.Where(x => x.Id == filter.RoleId);
        }

        if (filter.Name is not null)
        {
            query = query.Where(x => x.Name == filter.Name);
        }

        return query;
    }
}