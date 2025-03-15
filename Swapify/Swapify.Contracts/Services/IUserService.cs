using Swapify.Contracts.Models;

namespace Swapify.Contracts.Services;

public interface IUserService
{
    Task DeleteUserAsync(string userId);
    Task<bool> ExistsUserAsync(string userEmail);
    Task<IUser?> GetUserAsync(string userEmail);
    Task<IUser> GetUserAsync(string userEmail, string password);
    Task ChangeUserPasswordAsync(string userId, string password);
    Task<(int, IReadOnlyList<IUser>)> GetUsersAsync(int pageNumber, int pageSize, string? query = null);
    Task ConfirmUserEmailAsync(string userId);
    Task ForgotUserPasswordAsync(string userEmail);
    Task BlockUserAsync(string userId);
    Task UnblockUserAsync(string userId);
    Task ActiveUserAsync(string userId);
    Task InactiveUserAsync(string userId);
    Task<IUser> CreateUserAsync(string userEmail,
        string password,
        string? firstName,
        string? lastName);

    Task<IUser> CreateGoogleUserAsync(string userEmail,
        string? firstName,
        string? lastName);
    Task<IUser> ChangeUserNameAsync(string userId, string userName, string? firstName,
        string? lastName);
}