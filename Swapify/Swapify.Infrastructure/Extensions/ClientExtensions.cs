using Swapify.Contracts.Models;
using Swapify.Infrastructure.Entities;
using Swapify.Infrastructure.Models;

namespace Swapify.Infrastructure.Extensions;

public static class ClientExtensions
{
    public static IClient ToModel(this ClientEntity model)
    {
        return new Client()
        {
            ClientId = model.ClientId,
            ClientSecret = model.ClientSecretHash
        };
    }
}