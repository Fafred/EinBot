namespace EinBot.HostSetup;

using DataAccess.Data;
using DataAccess.DbAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;

public static partial class HostSetup
{
    public static IServiceCollection SetupDataAccess(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<ISqlDataAccess, SqlDataAccess>()
            .AddTransient<ITypesData, TypesData>()
            .AddTransient<IColumnDefinitionsData, ColumnDefinitionsData>()
            .AddTransient<ICollectionTypesData, CollectionTypesData>()
            .AddTransient<ITableDefinitionsData, TableDefinitionsData>()
            .AddTransient<ICellsData, CellsData>();


        return serviceCollection;
    }
}