﻿namespace EinBot.HostSetup;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static partial class HostSetup
{
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