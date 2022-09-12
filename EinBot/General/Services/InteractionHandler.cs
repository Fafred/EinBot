namespace EinBot.General.Services;

using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Reflection;

/// <summary>
/// This class handles all incoming interaction requests.
/// </summary>
public class InteractionHandler
{
    private readonly DiscordSocketClient _socketClient;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public InteractionHandler(
        DiscordSocketClient socketClient,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _socketClient = socketClient;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    /// <summary>
    /// Initializer which uses reflection to load all the interaction modules in the assembly.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        // Use reflection to load all the modules.
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

        _socketClient.InteractionCreated += OnInteractionCreated;
    }

    /// <summary>
    /// Called when the client receives an interaction message.  Finds the correct handler for the interaction and calls it.
    /// </summary>
    /// <param name="socketInteraction">The SocketInteraction created by Discord.NET</param>
    /// <returns></returns>
    private async Task OnInteractionCreated(SocketInteraction socketInteraction)
    {
        try
        {
            var context = new SocketInteractionContext(_socketClient, socketInteraction);
            await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
        } catch (Exception e)
        {
            // TODO: better logging.
            Console.WriteLine(e);

            // Delete the interaction throwing the exception.
            if (socketInteraction.Type == Discord.InteractionType.ApplicationCommand)
            {
                await socketInteraction.RespondAsync($"Error: {e}");
                await socketInteraction.GetOriginalResponseAsync()
                    .ContinueWith(async (message) => await message.Result.DeleteAsync());
            }
        }
    }
}
