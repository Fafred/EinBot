namespace EinBot.HostSetup;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A Class which holds extension methods for IServiceCollection to keep the IHost setup in Program clean.
/// </summary>
public static partial class HostSetup
{
    /// <summary>
    /// Extension method for IServiceCollection.  Sets up the configuration for the host.
    /// </summary>
    /// <param name="serviceCollection">This IServiceCollection object.</param>
    /// <param name="configuration">Out: the configuration object to write to.</param>
    /// <returns>This IServiceCollection.</returns>
    public static IServiceCollection SetupConfiguration(this IServiceCollection serviceCollection, out IConfiguration configuration)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
            .Build();

        configuration = config;
        return serviceCollection.AddSingleton(config);
    }
}