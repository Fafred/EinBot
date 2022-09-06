namespace EinBot.HostSetup;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using EinBot.General.Services;
using Microsoft.Extensions.DependencyInjection;

public static partial class HostSetup
{
    public static IServiceCollection SetupDiscordInteractionService(this IServiceCollection serviceCollection)
    {
        var interactionServiceConfig = new InteractionServiceConfig()
        {
            LogLevel = LogSeverity.Debug,
            UseCompiledLambda = true,
            DefaultRunMode = RunMode.Async
        };

        return serviceCollection
            .AddSingleton(x =>
                new InteractionService(
                    x.GetRequiredService<DiscordSocketClient>(),
                    interactionServiceConfig))
            .AddSingleton<InteractionHandler>();
    }
}
