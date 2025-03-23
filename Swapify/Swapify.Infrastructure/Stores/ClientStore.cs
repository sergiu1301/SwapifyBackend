using Microsoft.EntityFrameworkCore;
using Swapify.Contracts.Models;
using Swapify.Contracts.Stores;
using Swapify.Contracts.Transactions;
using Swapify.Infrastructure.Entities;
using Swapify.Infrastructure.Extensions;
using Validation;

namespace Swapify.Infrastructure.Stores;

public class ClientStore : IClientStore
{
    public async Task<IClient> GetAsync(string clientId, IAtomicScope atomicScope)
    {
        Requires.NotNullOrWhiteSpace(clientId, nameof(clientId));
        Requires.NotNull(atomicScope, nameof(atomicScope));

        ApplicationDbContext context =
            await atomicScope.ToDbContextAsync<ApplicationDbContext>(options => new ApplicationDbContext(options));

        ClientEntity? client = await context.Clients.Where(c => c.ClientId == clientId).FirstOrDefaultAsync();

        if (client == null)
        {
            throw new Exception();
        }

        return client.ToModel();
    }
}