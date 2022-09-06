namespace EinBot.HostSetup;

using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

public static partial class HostSetup
{
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
