namespace EinBot.HostSetup;

using Microsoft.Extensions.DependencyInjection;

public static partial class HostSetup
{
    public static IServiceCollection SetupDataAccess(this IServiceCollection serviceCollection)
    {
        return serviceCollection;
    }
}