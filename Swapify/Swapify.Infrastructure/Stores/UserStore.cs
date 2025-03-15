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

namespace Swapify.Infrastructure.Stores;

public class UserStore : IUserStore
{
    private readonly IContextService _contextService;

    public UserStore(IContextService contextService)
    {
        _contextService = contextService;
    }

    public async Task DeleteAsync(string userId, IAtomicScope atomicScope)
    {
        Requires.NotNull(userId, nameof(userId));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
                                        .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        user.Email = Guid.NewGuid() + "@gmail.com";
        user.UserName = "Removed User";

        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task<IUser?> GetAsync(IUserFilter filter, IAtomicScope atomicScope)
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
            return null;
        }

        return user!.ToModel();
    }

    public async Task<IUser> GetAsync(string userEmail, string passwordHash, IAtomicScope atomicScope)
    {
        Requires.NotNull(userEmail, nameof(userEmail));
        Requires.NotNull(passwordHash, nameof(passwordHash));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
                                              .ThenInclude(x => x.Role)
                                              .FirstOrDefaultAsync(u => u.Email == userEmail && u.PasswordHash == passwordHash);

        if (user == null)
        {
            throw new UserNameOrPasswordNotFoundException();
        }

        return user.ToModel();
    }

    public async Task ChangeUserPasswordAsync(string userId, string passwordHash, string salt,
        IAtomicScope atomicScope)
    {
        Requires.NotNull(userId, nameof(userId));
        Requires.NotNull(passwordHash, nameof(passwordHash));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
                                              .ThenInclude(x => x.Role)
                                              .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            throw new UserNameOrPasswordNotFoundException();
        }

        user.PasswordHash = passwordHash;
        user.Salt = salt;
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task<(int, IReadOnlyList<IUser>)> GetManyAsync(int pageNumber, int pageSize, IAtomicScope atomicScope, string? query = null)
    {
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        int startIndex = (pageNumber - 1) * pageSize;
        string currentUserEmail = await _contextService.GetCurrentContextAsync();

        IQueryable<UserEntity> queryable = context.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).AsQueryable();

        queryable = queryable.Where(u => u.Email != currentUserEmail && u.UserName != "Removed User");

        int numberUsers = await queryable.CountAsync();

        if (!string.IsNullOrEmpty(query))
        {
            queryable = queryable.Where(u => u.UserId.Contains(query) || u.UserName.Contains(query) || u.Email.Contains(query));
            numberUsers = await queryable.CountAsync();
        }

        IReadOnlyList<UserEntity> users = await queryable.Skip(startIndex)
                                                         .Take(pageSize)
                                                         .ToListAsync();

        var usersList = users.Select(u => u.ToModel()).ToList();

        return (numberUsers, usersList);
    }

    public async Task<string> GetUserSaltAsync(string userEmail, IAtomicScope atomicScope)
    {
        Requires.NotNull(userEmail, nameof(userEmail));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
                                              .FirstOrDefaultAsync(u => u.Email == userEmail);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        return user.Salt;
    }

    public async Task ConfirmUserEmailAsync(string userId, IAtomicScope atomicScope)
    {
        Requires.NotNull(userId, nameof(userId));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
                                              .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        user.EmailConfirmed = true;

        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task<IUser> BlockUserAsync(string userId, IAtomicScope atomicScope)
    {
        Requires.NotNull(userId, nameof(userId));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
                                              .ThenInclude(x => x.Role)
                                              .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        user.IsBlocked = true;

        context.Users.Update(user);
        await context.SaveChangesAsync();

        return user.ToModel();
    }

    public async Task<IUser> UnblockUserAsync(string userId, IAtomicScope atomicScope)
    {
        Requires.NotNull(userId, nameof(userId));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        user.IsBlocked = false;

        context.Users.Update(user);
        await context.SaveChangesAsync();

        return user.ToModel();
    }

    public async Task<bool> ExistsAsync(string userEmail, IAtomicScope atomicScope)
    {
        Requires.NotNull(userEmail, nameof(userEmail));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        bool userExists = await context.Users.Where(u => u.Email == userEmail)
                                             .AnyAsync();
        return userExists;
    }

    public async Task<IUser> CreateAsync(IUserDetail userDetail, IAtomicScope atomicScope)
    {
        Requires.NotNull(userDetail, nameof(userDetail));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity user = userDetail.ToEntity();

        bool userExists = await context.Users.Where(u => u.Email == user.Email)
                                             .AnyAsync();

        if (userExists)
        {
            throw new UserAlreadyExistsException();
        }

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user.ToModel();
    }

    public async Task<IUser> ChangeUserNameAsync(string userId, string userName, string? firstName,
        string? lastName, IAtomicScope atomicScope)
    {
        Requires.NotNull(userId, nameof(userId));
        Requires.NotNull(userName, nameof(userName));
        
        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        user.UserName = userName;
        user.FirstName = firstName;
        user.LastName = lastName;
        context.Users.Update(user);
        await context.SaveChangesAsync();

        return user.ToModel();
    }

    public async Task ActiveUserAsync(string userId, IAtomicScope atomicScope)
    {
        Requires.NotNull(userId, nameof(userId));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        user.IsOnline = true;

        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task InactiveUserAsync(string userId, IAtomicScope atomicScope)
    {
        Requires.NotNull(userId, nameof(userId));

        ApplicationDbContext context = await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        UserEntity? user = await context.Users.Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        user.IsOnline = false;

        context.Users.Update(user);
        await context.SaveChangesAsync();
    }
}