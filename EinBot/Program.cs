namespace EinBot;

using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Discord;

using EinBot.General.Services;
using EinBot.HostSetup;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public partial class Program
{
    /// <summary>
    /// The IServiceProvider which has been set up on boot.
    /// </summary>
    public static IServiceProvider ServiceProvider { get; private set; }

    public static Task Main() => /*new Program().Testing();*/ new Program().MainAsync();

    /// <summary>
    /// The app loop.  Sets up host configuration, connects services, and then starts the client.
    /// </summary>
    /// <returns></returns>
    public async Task MainAsync()
    {
        Console.WriteLine("Setting up Host.");

        IConfiguration configuration;

        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
                services
                    .SetupConfiguration(out configuration)
                    .SetupDiscordClient()
                    .SetupDiscordInteractionService()
                    .SetupDiscordCommandService()
                    .SetupDataAccess(configuration))
            .Build();

        ServiceProvider = host.Services;

        await StartClient(host);
    }

    /// <summary>
    /// Starts the Discord client, initializes the services, and logs in to Discord.
    /// </summary>
    /// <param name="host">The configured IHost.</param>
    /// <returns></returns>
    public async Task StartClient(IHost host)
    {
        using IServiceScope serviceScope = host.Services.CreateScope();

        IServiceProvider serviceProvider = serviceScope.ServiceProvider;
        IConfiguration config = serviceProvider.GetRequiredService<IConfigurationRoot>();
        DiscordSocketClient discordClient = serviceProvider.GetRequiredService<DiscordSocketClient>();

        await InitializeServices(serviceProvider, discordClient, config);

        await discordClient.LoginAsync(TokenType.Bot, config["BotToken"]);
        await discordClient.StartAsync();

        Console.WriteLine("Hello Discord.");

        await Task.Delay(Timeout.Infinite);

        Console.WriteLine("Good bye Discord.");
    }

    /// <summary>
    /// Initializes the Discord related services which need it, registers slash commands to any guild with an id in appsettings.json's "RegisterSlashCommandsTo"
    /// </summary>
    /// <param name="serviceProvider">The host's serviceProvider</param>
    /// <param name="discordClient">The Discord Client</param>
    /// <param name="config">The configuration object.</param>
    /// <returns></returns>
    public async Task InitializeServices(IServiceProvider serviceProvider, DiscordSocketClient discordClient, IConfiguration config)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var commandHandler = serviceProvider.GetRequiredService<CommandHandler>();
        var interactionHandler = serviceProvider.GetRequiredService<InteractionHandler>();

        var commandService = serviceProvider.GetRequiredService<CommandService>();
        var interactionService = serviceProvider.GetRequiredService<InteractionService>();

        discordClient.Log += OnLogAsync;
        commandService.Log += OnLogAsync;
        interactionService.Log += OnLogAsync;

        await commandHandler.InitializeAsync();
        await interactionHandler.InitializeAsync();

        discordClient.Ready += async () =>
        {
            // Register slash commands.
            List<string> guildIds = configuration.GetSection("RegisterSlashCommandsto").Get<List<string>>();

            foreach(string guildId in guildIds)
            {
                await interactionService.RegisterCommandsToGuildAsync(ulong.Parse(guildId));
            }

        };
    }

    public Task OnLogAsync(LogMessage message)
    {
        return Console.Out.WriteLineAsync(message.Exception?.ToString() ?? message.Message);
    }
}