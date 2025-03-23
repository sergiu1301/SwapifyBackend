namespace Swapify.Contracts.Models;

public interface IClient
{
    string ClientId { get; }

    string ClientSecret { get; }
}