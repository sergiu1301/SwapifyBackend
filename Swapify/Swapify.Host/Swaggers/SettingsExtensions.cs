using Swapify.Host.Settings;
using Validation;

namespace Swapify.Host.Swaggers
{
    public static class SettingsExtensions
    {
        public static ConfigurationManager AddSettingsConfigurations(
            this ConfigurationManager config)
        {
            Requires.NotNull(config, nameof(config));

            config
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            return config;
        }

        public static IServiceCollection AddSettings(
            this IServiceCollection services)
        {
            Requires.NotNull(services, nameof(services));

            services.AddOptions<TokenSettings>()
                .BindConfiguration(nameof(TokenSettings));
            services.AddOptions<SwapifySettings>()
                .BindConfiguration(nameof(SwapifySettings));
            services.AddOptions<ConnectionStrings>()
                .BindConfiguration(nameof(ConnectionStrings));

            return services;
        }
    }
}
