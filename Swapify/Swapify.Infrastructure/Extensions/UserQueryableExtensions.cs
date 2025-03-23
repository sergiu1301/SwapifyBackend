using Swapify.Contracts.Filters;
using Swapify.Infrastructure.Entities;
using Validation;

namespace Swapify.Infrastructure.Extensions;

public static class UserQueryableExtensions
{
    public static IQueryable<UserEntity> ApplyFiltering(
        this IQueryable<UserEntity> query,
        IUserFilter filter)
    {
        Requires.NotNull(query, nameof(query));
        Requires.NotNull(filter, nameof(filter));

        if (filter.UserId is not null)
        {
            query = query.Where(x => x.Id == filter.UserId);
        }

        if (filter.Email is not null)
        {
            query = query.Where(x => x.Email == filter.Email);
        }

        return query;
    }
}