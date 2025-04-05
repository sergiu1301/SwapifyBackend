using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Swapify.Contracts.Models;
using Swapify.Contracts.Services;
using Swapify.Contracts.Stores;
using Swapify.Contracts.Transactions;
using Swapify.Infrastructure.Entities;
using System.Text;
using Swapify.Infrastructure.Exceptions;
using System.Web;
using Swapify.Contracts.Filters;
using Swapify.Infrastructure.Filters;
using Validation;

namespace Swapify.Infrastructure.Services;

public class UserManagerService : IUserManagerService
{
    private readonly IUserStore _userStore;
    private readonly UserManager<UserEntity> _userManager;
    private readonly RoleManager<RoleEntity> _roleManager;
    private readonly IClientStore _clientStore;
    private readonly IAtomicScopeFactory _atomicScopeFactory;
    private readonly IEmailNotificationService _emailNotificationService;

    private const string DefaultRole = "User";

    public UserManagerService(UserManager<UserEntity> userManager, RoleManager<RoleEntity> roleManager,
        IClientStore clientStore, IUserStore userStore, IEmailNotificationService emailNotificationService, IAtomicScopeFactory atomicScopeFactory)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _clientStore = clientStore;
        _userStore = userStore;
        _emailNotificationService = emailNotificationService;
        _atomicScopeFactory = atomicScopeFactory;
    }

    public async Task RegisterAsync(string userEmail, string userPassword, string clientId, string clientSecret)
    {
        Requires.NotNullOrEmpty(userEmail, nameof(userEmail));
        Requires.NotNullOrEmpty(userPassword, nameof(userPassword));
        Requires.NotNullOrEmpty(clientId, nameof(clientId));
        Requires.NotNullOrEmpty(clientSecret, nameof(clientSecret));

        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        IClient client = await _clientStore.GetAsync(clientId, atomicScope);

        if (!VerifySecret(clientSecret, client.ClientSecret))
        {
            throw new ClientNotSupportedException();
        }

        UserEntity? userEntity = await _userManager.FindByEmailAsync(userEmail);

        if (userEntity != null)
        {
            throw new UserAlreadyExistsException();
        }

        RoleEntity? roleEntity = await _roleManager.FindByNameAsync(DefaultRole);

        if (roleEntity == null)
        {
            throw new RoleNotFoundException();
        }

        UserEntity user = new UserEntity
        {
            UserName = userEmail.Split('@')[0],
            Email = userEmail,
            CreatedBy = clientId,
            UserRoles = new List<UserRoleEntity>
            {
                new() { RoleId = roleEntity.Id }
            }
        };

        IdentityResult result = await _userManager.CreateAsync(user, userPassword);

        if (!result.Succeeded)
        {
            throw new UserCannotBeCreatedException();
        }

        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        string encodedToken = HttpUtility.UrlEncode(token);

        await _emailNotificationService.SendEmailConfirmationAsync(user.UserName, user.Id, userEmail, encodedToken);
    }

    public async Task ForgotPasswordAsync(string userEmail, string clientId, string clientSecret)
    {
        Requires.NotNullOrEmpty(userEmail, nameof(userEmail));
        Requires.NotNullOrEmpty(clientId, nameof(clientId));
        Requires.NotNullOrEmpty(clientSecret, nameof(clientSecret));

        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        IClient client = await _clientStore.GetAsync(clientId, atomicScope);

        if (!VerifySecret(clientSecret, client.ClientSecret))
        {
            throw new ClientNotSupportedException();
        }

        UserEntity? userEntity = await _userManager.FindByEmailAsync(userEmail);

        if (userEntity == null)
        {
            throw new UserNotFoundException();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(userEntity);
        var encodedToken = HttpUtility.UrlEncode(token);

        await _emailNotificationService.SendForgotPasswordAsync(userEntity.UserName, userEntity.Id, userEmail, encodedToken);
    }

    public async Task ResetPasswordAsync(string userId, string token, string newPassword, string clientId,
        string clientSecret)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));
        Requires.NotNullOrEmpty(token, nameof(token));
        Requires.NotNullOrEmpty(newPassword, nameof(newPassword));
        Requires.NotNullOrEmpty(clientId, nameof(clientId));
        Requires.NotNullOrEmpty(clientSecret, nameof(clientSecret));

        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        IClient client = await _clientStore.GetAsync(clientId, atomicScope);

        if (!VerifySecret(clientSecret, client.ClientSecret))
        {
            throw new ClientNotSupportedException();
        }

        IUserFilter userFilter = new UserFilter()
        {
            UserId = userId
        };

        await _userStore.ResetPasswordAsync(userFilter, token, newPassword, atomicScope);
    }

    public async Task ConfirmEmailAsync(string userId, string token)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));
        Requires.NotNullOrEmpty(token, nameof(token));

        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        IUserFilter userFilter = new UserFilter()
        {
            UserId = userId
        };

        await _userStore.ConfirmEmailAsync(userFilter, token, atomicScope);
    }

    public async Task<IUser> VerifyAsync(string userEmail, string userPassword, string clientId, string clientSecret)
    {
        Requires.NotNullOrEmpty(userEmail, nameof(userEmail));
        Requires.NotNullOrEmpty(userPassword, nameof(userPassword));
        Requires.NotNullOrEmpty(clientId, nameof(clientId));
        Requires.NotNullOrEmpty(clientSecret, nameof(clientSecret));

        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        IClient client = await _clientStore.GetAsync(clientId, atomicScope);

        if (!VerifySecret(clientSecret, client.ClientSecret))
        {
            throw new ClientNotSupportedException();
        }

        IUserFilter userFilter = new UserFilter()
        {
            Email = userEmail
        };

        IUser user = await _userStore.CheckPasswordAsync(userFilter, userPassword, atomicScope);

        return user;
    }

    public async Task BlockAsync(string userId)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));

        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        IUserFilter userFilter = new UserFilter()
        {
            UserId = userId
        };

        IUser user = await _userStore.BlockUserAsync(userFilter, atomicScope);

        await _emailNotificationService.SendBlockAccountAsync(user.UserName, user.Email);
    }

    public async Task DeleteAsync(string userId)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));

        await using IAtomicScope atomicScope = _atomicScopeFactory.Create();

        IUserFilter userFilter = new UserFilter()
        {
            UserId = userId
        };

        await _userStore.DeleteAsync(userFilter, atomicScope);

        await atomicScope.CommitAsync();
    }

    public async Task<IUser> GetByEmailAsync(string userEmail)
    {
        Requires.NotNullOrEmpty(userEmail, nameof(userEmail));

        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        UserFilter filter = new()
        {
            Email = userEmail
        };

        return await _userStore.GetAsync(filter, atomicScope);
    }

    public async Task<IUser> GetByIdAsync(string userId)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));

        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        UserFilter filter = new()
        {
            UserId = userId
        };

        return await _userStore.GetAsync(filter, atomicScope);
    }

    public async Task<(int, IReadOnlyList<IUser>)> GetManyAsync(int pageNumber, int pageSize, string? query = null)
    {
        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        return await _userStore.GetManyAsync(pageNumber, pageSize, atomicScope, query);
    }

    public async Task UpdateUserAsync(string userId, string? requestFirstName, string? requestLastName, string? requestPhoneNumber,
        string? requestUserName)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));

        UserEntity? userEntity = await _userManager.FindByIdAsync(userId);

        if (userEntity == null)
        {
            throw new UserNotFoundException();
        }

        bool hasChanges = false;

        if (!string.IsNullOrWhiteSpace(requestFirstName) && requestFirstName != userEntity.FirstName)
        {
            userEntity.FirstName = requestFirstName;
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(requestLastName) && requestLastName != userEntity.LastName)
        {
            userEntity.LastName = requestLastName;
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(requestPhoneNumber) && requestPhoneNumber != userEntity.PhoneNumber)
        {
            userEntity.PhoneNumber = requestPhoneNumber;
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(requestUserName) && requestUserName != userEntity.UserName)
        {
            userEntity.UserName = requestUserName;
            hasChanges = true;
        }

        if (hasChanges)
        {
            IdentityResult result = await _userManager.UpdateAsync(userEntity);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to update user.");
            }
        }
    }

    public async Task UnblockAsync(string userId)
    {
        Requires.NotNullOrEmpty(userId, nameof(userId));

        await using IAtomicScope atomicScope = _atomicScopeFactory.CreateWithoutTransaction();

        IUserFilter userFilter = new UserFilter()
        {
            UserId = userId
        };

        IUser user = await _userStore.UnblockUserAsync(userFilter, atomicScope);

        await _emailNotificationService.SendUnblockAccountAsync(user.UserName, user.Email);

        await atomicScope.CommitAsync();
    }

    private bool VerifySecret(string inputSecret, string storedHash)
    {
        using var sha256 = SHA256.Create();
        var inputHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(inputSecret)));
        return inputHash == storedHash;
    }
}