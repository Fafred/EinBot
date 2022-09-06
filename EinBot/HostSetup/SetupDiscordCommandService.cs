namespace EinBot.HostSetup;

using Discord;
using Discord.Commands;
using EinBot.General.Services;
using Microsoft.Extensions.DependencyInjection;

public static partial class HostSetup
{
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
