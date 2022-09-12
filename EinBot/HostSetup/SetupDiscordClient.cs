namespace EinBot.HostSetup;

using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

public static partial class HostSetup
{
    /// <summary>
    /// An IServiceCollection extension for setting up the Discord client.
    /// </summary>
    /// <param name="serviceCollection">This IServiceCollection.</param>
    /// <returns>This IServiceCollection.</returns>
    public static IServiceCollection SetupDiscordClient(this IServiceCollection serviceCollection)
    {
        var socketConfig = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All,
            AlwaysDownloadUsers = true,
            MessageCacheSize = 200,
            LogLevel = LogSeverity.Debug,
        };

        return serviceCollection.AddSingleton(x => new DiscordSocketClient(socketConfig));
    }
}
