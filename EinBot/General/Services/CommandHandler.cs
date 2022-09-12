namespace EinBot.General.Services;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Reflection;

/// <summary>
/// Class for handling any prefix commands.
/// </summary>
public class CommandHandler
{
    private readonly DiscordSocketClient _socketClient;
    private readonly CommandService _commandService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    private string _prefix = "";

    public CommandHandler(
        DiscordSocketClient socketClient,
        CommandService commandService,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _socketClient = socketClient;
        _commandService = commandService;
        _serviceProvider = serviceProvider;
        _configuration = configuration;

        _prefix = _configuration["Prefix"];
    }

    /// <summary>
    /// Loads the prefix command modules and connects to the Discord client to listen to messages.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        // Use reflection to load all the modules.
        await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

        _socketClient.MessageReceived += OnMessageReceived;
    }

    /// <summary>
    /// Executes whenever a message is sent in a channel the bot is in.  Checks the beginning of the message for
    ///     the configured prefix or an @ to the bot and then calls ExecuteAsync, passing along the command.
    /// </summary>
    /// <param name="socketMessage"></param>
    /// <returns></returns>
    private async Task OnMessageReceived(SocketMessage socketMessage)
    {
        // Some gatekeeping.
        if (socketMessage is not SocketUserMessage message) return;

        // No responding to bots.
        if (message.Source is not MessageSource.User) return;

        int argPos = 0;

        if (!message.HasStringPrefix(_prefix, ref argPos) && !message.HasMentionPrefix(_socketClient.CurrentUser, ref argPos)) return;
        
        try
        {
            var context = new SocketCommandContext(_socketClient, message);

            await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
        } catch (Exception e)
        {
            // TODO: Better logging.
            Console.WriteLine(e);
        }
    }
}
