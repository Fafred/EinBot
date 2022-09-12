namespace EinBot.HostSetup;

using Discord;
using Discord.Commands;
using EinBot.General.Services;
using Microsoft.Extensions.DependencyInjection;

public static partial class HostSetup
{
    /// <summary>
    /// An IServiceCollection extension method for setting up the Prefix Command service for the Discord Client.
    /// </summary>
    /// <param name="serviceCollection">This IServiceCollection.</param>
    /// <returns>This IServiceCollection.</returns>
    public static IServiceCollection SetupDiscordCommandService(this IServiceCollection serviceCollection)
    {
        var commandServiceConfig = new CommandServiceConfig()
        {
            LogLevel = LogSeverity.Debug,
            CaseSensitiveCommands = false,
            DefaultRunMode = RunMode.Async
        };

        return serviceCollection
                .AddSingleton(x =>
                    new CommandService(commandServiceConfig))
                .AddSingleton<CommandHandler>();
    }
}
