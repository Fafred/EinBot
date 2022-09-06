namespace EinBot;

using EinBot.HostSetup;
using Microsoft.Extensions.Hosting;

public partial class Program
{
    public static Task Main() => new Program().MainAsync();

    public async Task MainAsync()
    {
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
                services
                    .SetupConfiguration()
                    .SetupDiscordClient()
                    .SetupDiscordInteractionService()
                    .SetupDiscordCommandService())
            .Build();

        await StartClient(host);
    }
}