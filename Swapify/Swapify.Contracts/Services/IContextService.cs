namespace Swapify.Contracts.Services;

public interface IContextService
{
    Task<string> GetCurrentContextAsync();
    Task<string> GetCurrentGoogleFirstNameAsync();
    Task<string> GetCurrentGoogleLastNameAsync();
    Task<string> GetCurrentUserIdAsync();
}