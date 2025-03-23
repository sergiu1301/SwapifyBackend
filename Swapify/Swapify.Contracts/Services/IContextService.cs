namespace Swapify.Contracts.Services;

public interface IContextService
{
    Task<string?> GetCurrentContextAsync();
    Task<string?> GetCurrentUserIdAsync();
}