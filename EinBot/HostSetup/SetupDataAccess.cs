namespace EinBot.HostSetup;

using EinBotDB.Context;
using EinBotDB.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static partial class HostSetup
{
    public static IServiceCollection SetupDataAccess(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection
            .AddDbContextFactory<EinDataContext>(options => options.UseSqlite(configuration.GetConnectionString("EinBotDb")))
            .AddSingleton<IEinDataAccess, EinDataAccess>();

        return serviceCollection;
    }
}