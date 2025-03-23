using Swapify.Contracts.Models;

namespace Swapify.Infrastructure.Models;

public class Client : IClient
{
    public string ClientId { get; set; }

    public string ClientSecret { get; set; }
}