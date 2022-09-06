namespace EinBot;

using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using EinBot.General.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public partial class Program
{   
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

        // Register slash commands.

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
