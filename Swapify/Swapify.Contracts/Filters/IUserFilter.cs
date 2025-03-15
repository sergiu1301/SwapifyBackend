namespace Swapify.Contracts.Filters;

public interface IUserFilter
{
    string? UserId { get; }

    string? Email { get; }
}