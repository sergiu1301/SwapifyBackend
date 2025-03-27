using Microsoft.EntityFrameworkCore;
using Swapify.Contracts.Filters;
using Swapify.Contracts.Models;
using Swapify.Contracts.Services;
using Swapify.Contracts.Stores;
using Swapify.Contracts.Transactions;
using Swapify.Infrastructure.Entities;
using Swapify.Infrastructure.Exceptions;
using Validation;
using Swapify.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace Swapify.Infrastructure.Stores;

public class UserStore : IUserStore
{
    private readonly IContextService _contextService;
    private readonly UserManager<UserEntity> _userManager;

    public UserStore(IContextService contextService, UserManager<UserEntity> userManager)
    {
        _contextService = contextService;
        _userManager = userManager;
    }

    public async Task DeleteAsync(IUserFilter filter, IAtomicScope atomicScope)
    {
        Requires.NotNull(filter, nameof(filter));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
                                              .ApplyFiltering(filter)
                                              .FirstOrDefaultAsync();

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        IdentityResult result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
            throw new UserCannotBeDeletedException();
    }

    public async Task<IUser> GetAsync(IUserFilter filter, IAtomicScope atomicScope)
    {
        Requires.NotNull(filter, nameof(filter));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
                                              .ThenInclude(x => x.Role)
                                              .ApplyFiltering(filter)
                                              .FirstOrDefaultAsync();

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        return user.ToModel();
    }

    public async Task ResetPasswordAsync(IUserFilter filter, string token, string newPassword, IAtomicScope atomicScope)
    {
        Requires.NotNull(filter, nameof(filter));
        Requires.NotNull(token, nameof(token));
        Requires.NotNull(newPassword, nameof(newPassword));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.ApplyFiltering(filter)
                                              .FirstOrDefaultAsync();

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        IdentityResult result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
            throw new PasswordCannotBeChangedException();
    }

    public async Task<(int, IReadOnlyList<IUser>)> GetManyAsync(int pageNumber, int pageSize, IAtomicScope atomicScope, string? query = null)
    {
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        int startIndex = (pageNumber - 1) * pageSize;
        string currentUserId = await _contextService.GetCurrentUserIdAsync();

        IQueryable<UserEntity> queryable = context.Users.AsQueryable()
                                                        .Include(c => c.UserRoles)
                                                        .ThenInclude(c => c.Role)
                                                        .Where(u => u.Id != currentUserId && u.UserName != "Removed User");

        int numberUsers = await queryable.CountAsync();

        if (!string.IsNullOrEmpty(query))
        {
            queryable = queryable.Where(u => u.Email.Contains(query));
            numberUsers = await queryable.CountAsync();
        }

        IReadOnlyList<UserEntity> users = await queryable.Skip(startIndex)
                                                         .Take(pageSize)
                                                         .ToListAsync();

        var usersList = users.Select(u => u.ToModel()).ToList();

        return (numberUsers, usersList);
    }

    public async Task<IUser> BlockUserAsync(IUserFilter filter, IAtomicScope atomicScope)
    {
        Requires.NotNull(filter, nameof(filter));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
                                              .ThenInclude(x => x.Role)
                                              .ApplyFiltering(filter)
                                              .FirstOrDefaultAsync();

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        await _userManager.UpdateAsync(user);

        return user.ToModel();
    }

    public async Task<IUser> UnblockUserAsync(IUserFilter filter, IAtomicScope atomicScope)
    {
        Requires.NotNull(filter, nameof(filter));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .ApplyFiltering(filter)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        await _userManager.SetLockoutEndDateAsync(user, null);
        await _userManager.ResetAccessFailedCountAsync(user);
        await _userManager.UpdateAsync(user);

        return user.ToModel();
    }

    public async Task ConfirmEmailAsync(IUserFilter filter, string token, IAtomicScope atomicScope)
    {
        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
                                              .ThenInclude(x => x.Role)
                                              .ApplyFiltering(filter)
                                              .FirstOrDefaultAsync();

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        if (user.EmailConfirmed)
        {
            throw new EmailAlreadyConfirmedException();
        }

        IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
        {
            throw new EmailCannotBeConfirmedException();
        }
    }

    public async Task<IUser> CheckPasswordAsync(IUserFilter filter, string userPassword, IAtomicScope atomicScope)
    {
        Requires.NotNull(filter, nameof(filter));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .ApplyFiltering(filter)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        if (!await _userManager.CheckPasswordAsync(user, userPassword))
        {
            throw new UserNameOrPasswordNotFoundException();
        }

        if (user.LockoutEnd != null)
        {
            throw new UserNameOrPasswordNotFoundException();
        }

        if (!user.EmailConfirmed)
        {
            throw new EmailNotConfirmedException();
        }

        return user.ToModel();
    }
}