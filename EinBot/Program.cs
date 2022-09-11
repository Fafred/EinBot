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
using EinBotDB.DataAccess;
using System.Text;
using Microsoft.EntityFrameworkCore;

public partial class Program
{
    public static IServiceProvider ServiceProvider { get; private set; }

    public static Task Main() => new Program().Testing();

    public async Task Testing()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
                services
                    .AddDbContextFactory<EinDataContext>(options => options.UseSqlite(configuration.GetConnectionString("EinBotDb")))
                    .AddSingleton<EinDataAccess>())
            .Build();


        ServiceProvider = host.Services;

        var dal = ServiceProvider.GetRequiredService<EinDataAccess>();


        Dictionary<string, string> columnsDataDict = new Dictionary<string, string>()
        {
            {"WholeNumber", "100"},
            {"TextField", "This is a new text field." },
            {"ListOfStrings", "One element in a list." }
        };

        dal.CreateTable("GoldTable", CollectionTypesEnum.PerKey);
        dal.CreateColumn(2, "Gold", DataTypesEnum.Int);

        dal.AddRow(2, "SoAndSo", new Dictionary<string, string>() { { "Gold", "41" } });

        var table = dal.GetEinTable(2);

        foreach(var row in table.Rows)
        {
            Console.WriteLine(row);
        }


        Console.WriteLine("---");


        Task.Delay(-1);
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