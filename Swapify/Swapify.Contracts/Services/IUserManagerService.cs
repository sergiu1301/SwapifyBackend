using Swapify.Contracts.Models;

namespace Swapify.Contracts.Services;

public interface IUserManagerService
{
    Task RegisterAsync(string userEmail, string userPassword, string clientId, string clientSecret);
    Task<IUser> VerifyAsync(string userEmail, string userPassword, string clientId, string clientSecret);
    Task ForgotPasswordAsync(string userEmail, string clientId, string clientSecret);
    Task ResetPasswordAsync(string userId, string token, string newPassword, string clientId, string clientSecret);
    Task ConfirmEmailAsync(string userId, string token);
    Task UnblockAsync(string userId);
    Task BlockAsync(string userId);
    Task DeleteAsync(string userId);
    Task<IUser> GetByEmailAsync(string userEmail);
    Task<IUser> GetByIdAsync(string userId);
    Task<(int, IReadOnlyList<IUser>)> GetManyAsync(int pageNumber, int pageSize, string? query = null);
    Task UpdateUserAsync(string userId, string? requestFirstName, string? requestLastName, string? requestPhoneNumber, string? requestUserName);
}