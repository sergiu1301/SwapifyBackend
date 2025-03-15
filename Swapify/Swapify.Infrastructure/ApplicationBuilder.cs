using Microsoft.Extensions.DependencyInjection;

namespace Swapify.Infrastructure;

public class ApplicationBuilder
{
    public IServiceCollection Services { get; }

    public ApplicationBuilder(IServiceCollection services)
    {
        Services = services;
    }
}