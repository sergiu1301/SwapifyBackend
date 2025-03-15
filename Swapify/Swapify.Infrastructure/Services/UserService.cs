using Swapify.Contracts.Models;
using Swapify.Contracts.Services;
using Swapify.Contracts.Stores;
using Swapify.Contracts.Transactions;
using Swapify.Infrastructure.Exceptions;
using Swapify.Infrastructure.Filters;
using Swapify.Infrastructure.Models;
using Swapify.Notifications.Events;
using Swapify.Notifications.Handlers;
using Swapify.Notifications.Models;
using Validation;

namespace Swapify.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserStore _userStore;
    private readonly IUserRoleService _userRoleService;
    private readonly IAtomicScopeFactory _atomicScopeFactory;
    private readonly EmailEventPublisher _emailEventPublisher;
    private readonly EmailHandler _emailHandler;
    private readonly IEncryptDecryptService _encryptDecryptService;

    public UserService(IUserStore userStore, IUserRoleService userRoleService, IEncryptDecryptService encryptDecryptService, IAtomicScopeFactory atomicScopeFactory, EmailEventPublisher emailEventPublisher, EmailHandler emailHandler)
    {
        _userStore = userStore;
        _userRoleService = userRoleService;
        _atomicScopeFactory = atomicScopeFactory;
        _encryptDecryptService = encryptDecryptService;
        _emailEventPublisher = emailEventPublisher;
        _emailHandler = emailHandler;

        _emailEventPublisher.EmailEvent += _emailHandler.HandleEmailEvent;
    }

    public async Task DeleteUserAsync(string userId)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));

        await using IAtomicScope atomicScope = _atomicScopeFactory.Create();

        await _userStore.DeleteAsync(userId, atomicScope);

        await atomicScope.CommitAsync();
    }

    public async Task<IUser?> GetUserAsync(string userEmail)
    {
        Requires.NotNullOrEmpty(userEmail, nameof(userEmail));

        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        UserFilter filter = new()
        {
            Email = userEmail
        };

        return await _userStore.GetAsync(filter, atomicScope);
    }

    public async Task<bool> ExistsUserAsync(string userEmail)
    {
        Requires.NotNullOrEmpty(userEmail, nameof(userEmail));

        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        return await _userStore.ExistsAsync(userEmail, atomicScope);
    }

    public async Task<IUser> GetUserAsync(string userEmail, string password)
    {
        Requires.NotNullOrEmpty(userEmail, nameof(userEmail));
        Requires.NotNullOrEmpty(password, nameof(password));

        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        string salt = await _userStore.GetUserSaltAsync(userEmail, atomicScope);
        string hashPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return await _userStore.GetAsync(userEmail, hashPassword, atomicScope);
    }

    public async Task ChangeUserPasswordAsync(string userId, string password)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));
        Requires.NotNullOrEmpty(password, nameof(password));

        await using IAtomicScope atomicScope = _atomicScopeFactory.Create();

        string decryptedUserId = _encryptDecryptService.Decrypt(userId);

        string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        string hashPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        await _userStore.ChangeUserPasswordAsync(decryptedUserId, hashPassword, salt, atomicScope);

        await atomicScope.CommitAsync();
    }

    public async Task<(int, IReadOnlyList<IUser>)> GetUsersAsync(int pageNumber, int pageSize, string? query = null)
    {
        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        return await _userStore.GetManyAsync(pageNumber, pageSize, atomicScope, query);
    }

    public async Task ConfirmUserEmailAsync(string userId)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));

        await using IAtomicScope atomicScope = _atomicScopeFactory.Create();

        string decryptedUserId = _encryptDecryptService.Decrypt(userId);

        await _userStore.ConfirmUserEmailAsync(decryptedUserId, atomicScope);

        await atomicScope.CommitAsync();
    }

    public async Task ForgotUserPasswordAsync(string userEmail)
    {
        Requires.NotNullOrEmpty(userEmail, nameof(userEmail));

        await using IAtomicScope atomicScope = _atomicScopeFactory.Create();

        UserFilter filter = new()
        {
            Email = userEmail
        };

        IUser user = await _userStore.GetAsync(filter, atomicScope);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        await SendForgotPasswordAsync(user.FirstName, user.LastName, user.Email, user.UserId);

        await atomicScope.CommitAsync();
    }

    public async Task BlockUserAsync(string userId)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));

        await using IAtomicScope atomicScope = _atomicScopeFactory.Create();

        IUser user = await _userStore.BlockUserAsync(userId, atomicScope);

        await SendBlockAccountAsync(user.FirstName, user.LastName, user.Email);

        await atomicScope.CommitAsync();
    }

    public async Task UnblockUserAsync(string userId)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));

        await using IAtomicScope atomicScope = _atomicScopeFactory.Create();

        IUser user = await _userStore.UnblockUserAsync(userId, atomicScope);

        await SendUnblockAccountAsync(user.FirstName, user.LastName, user.Email);

        await atomicScope.CommitAsync();
    }

    public async Task ActiveUserAsync(string userId)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));
        await using IAtomicScope atomicScope = _atomicScopeFactory.Create();

        await _userStore.ActiveUserAsync(userId, atomicScope);

        await atomicScope.CommitAsync();
    }

    public async Task InactiveUserAsync(string userId)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));
        await using IAtomicScope atomicScope = _atomicScopeFactory.Create();

        await _userStore.InactiveUserAsync(userId, atomicScope);

        await atomicScope.CommitAsync();
    }

    public async Task<IUser> CreateUserAsync(string userEmail,
        string password,
        string? firstName,
        string? lastName)
    {
        Requires.NotNullOrEmpty(userEmail, nameof(userEmail));
        Requires.NotNullOrEmpty(password, nameof(password));

        await using IAtomicScope atomicScope = _atomicScopeFactory.Create();

        string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        string hashPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        IUserDetail userDetail = new UserDetail()
        {
            UserId = Guid.NewGuid().ToString(),
            UserName = userEmail.Split('@')[0],
            Email = userEmail,
            PasswordHash = hashPassword,
            Salt = salt,
            FirstName = firstName,
            LastName = lastName
        };

        IUser user = await _userStore.CreateAsync(userDetail, atomicScope);

        await _userRoleService.AddUserRoleAsync(user.UserId, Roles.UserRole, atomicScope);

        await SendEmailConfirmationAsync(firstName, lastName, userEmail, user.UserId);

        await atomicScope.CommitAsync();

        return user;
    }

    public async Task<IUser> CreateGoogleUserAsync(string userEmail,
        string? firstName,
        string? lastName)
    {
        Requires.NotNullOrEmpty(userEmail, nameof(userEmail));

        await using IAtomicScope atomicScope = _atomicScopeFactory.Create();

        IUserDetail userDetail = new UserDetail()
        {
            UserId = Guid.NewGuid().ToString(),
            UserName = userEmail.Split('@')[0],
            Email = userEmail,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true
        };

        IUser user = await _userStore.CreateAsync(userDetail, atomicScope);

        await _userRoleService.AddUserRoleAsync(user.UserId, Roles.UserRole, atomicScope);

        await atomicScope.CommitAsync();

        return user;
    }

    public async Task<IUser> ChangeUserNameAsync(string userId, string userName, string? firstName,
        string? lastName)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));
        Requires.NotNullOrEmpty(userName, nameof(userName));

        await using IAtomicScope atomicScope = _atomicScopeFactory.Create();

        IUser user = await _userStore.ChangeUserNameAsync(userId, userName, firstName, lastName, atomicScope);

        await atomicScope.CommitAsync();

        return user;
    }

    private async Task SendEmailConfirmationAsync(string? firstName, string? lastName, string userEmail, string userId)
    {
        string user = _encryptDecryptService.Encrypt(userId);

        string redirectUrl = $"http://localhost:5173/confirm_email?userId={user}";

        IAttachments emailEvent = new Attachments()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = userEmail,
            RedirectUrl = redirectUrl,
            Subject = "Email Confirmation"
        };
        
        _emailEventPublisher.RaiseEmailEvent(emailEvent, "EmailConfirmationTemplate.html");
    }

    private async Task SendForgotPasswordAsync(string? firstName, string? lastName, string userEmail, string userId)
    {
        string user = _encryptDecryptService.Encrypt(userId);
        
        string redirectUrl = $"http://localhost:5173/set_new_password?userId={user}";

        IAttachments emailEvent = new Attachments()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = userEmail,
            RedirectUrl = redirectUrl,
            Subject = "Forgot Password"
        };

        _emailEventPublisher.RaiseEmailEvent(emailEvent, "ForgotPasswordTemplate.html");
    }

    private async Task SendBlockAccountAsync(string? firstName, string? lastName, string userEmail)
    {
        IAttachments emailEvent = new Attachments()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = userEmail,
            Subject = "Block Account"
        };

        _emailEventPublisher.RaiseEmailEvent(emailEvent, "BlockAccountTemplate.html");
    }

    private async Task SendUnblockAccountAsync(string? firstName, string? lastName, string userEmail)
    {
        IAttachments emailEvent = new Attachments()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = userEmail,
            Subject = "Unblock Account"
        };

        _emailEventPublisher.RaiseEmailEvent(emailEvent, "UnblockAccountTemplate.html");
    }
}