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
/// 
/// 
using EinBotDB.Context;
using EinBotDB.Exceptions;
using EinBotDB.DataAccess;
using System.Text;

public partial class Program
{
    public static IServiceProvider ServiceProvider { get; private set; }

    public static Task Main() => new Program().Testing();

    public async Task Testing()
    {
        var fact = new EinDataContextFactory();
        var context = fact.CreateDbContext(new string[] { });
        EinDataAccess da = new EinDataAccess(fact);

        EinTable table = da.GetEinTable("Test");

        foreach(var columnName in table.ColumnDataTypes.Keys)
        {
            Console.WriteLine($"{columnName} : {table.ColumnDataTypes[columnName]}\n+");
        }

        StringBuilder builder = new StringBuilder();

        foreach(var row in table.Rows)
        {
            builder.AppendLine(row.ToString());
        }

        Console.WriteLine(builder.ToString());
    }



    public async Task MainAsync()
    {
        Console.WriteLine("Setting up Host.");
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
                services
                    .SetupConfiguration()
                    .SetupDiscordClient()
                    .SetupDiscordInteractionService()
                    .SetupDiscordCommandService()
                    .SetupDataAccess())
            .Build();

        ServiceProvider = host.Services;

        await StartClient(host);
    }

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

    public async Task InitializeServices(IServiceProvider serviceProvider, DiscordSocketClient discordClient, IConfiguration config)
    {
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
            await interactionService.RegisterCommandsToGuildAsync(ulong.Parse(config["TestGuildID"]));
        };
    }

    public Task OnLogAsync(LogMessage message)
    {
        return Console.Out.WriteLineAsync(message.Exception?.ToString() ?? message.Message);
    }
}