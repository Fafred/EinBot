namespace EinBot.HostSetup;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using EinBot.General.Services;
using Microsoft.Extensions.DependencyInjection;

public static partial class HostSetup
{
    /// <summary>
    /// An IServiceCollection extension for setting up the Interaction Services for the Discord Client.
    /// </summary>
    /// <param name="serviceCollection">This IServiceCollection.</param>
    /// <returns>This IServiceCollection.</returns>
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
