namespace EinBotDB.Context;

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class EinDataContextFactory : IDesignTimeDbContextFactory<EinDataContext>
{
    public EinDataContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
            .Build();

        var options = new DbContextOptionsBuilder()
            .UseSqlite(configuration.GetConnectionString("EinBotDB"))
            .Options;

        return new EinDataContext(options);
    }
}
