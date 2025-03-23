namespace Swapify.Contracts.Services;

public interface IContextService
{
    Task<string?> GetCurrentUserIdAsync();
}