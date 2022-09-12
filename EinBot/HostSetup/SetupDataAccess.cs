namespace EinBot.HostSetup;

using EinBotDB.Context;
using EinBotDB.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static partial class HostSetup
{
    /// <summary>
    /// Extension method for IServiceCollection for setting up any databases and access to them.
    /// </summary>
    /// <param name="serviceCollection">This IServiceCollection.</param>
    /// <param name="configuration">The previously built IConfiguration object.</param>
    /// <returns>This IServiceCollection.</returns>
    public static IServiceCollection SetupDataAccess(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection
            .AddDbContextFactory<EinDataContext>(options => options.UseSqlite(configuration.GetConnectionString("EinBotDb")))
            .AddSingleton<IEinDataAccess, EinDataAccess>();

        return serviceCollection;
    }
}