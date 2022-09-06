namespace EinBot.General.Services;

using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Reflection;

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

    public async Task InitializeAsync()
    {
        // Use reflection to load all the modules.
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

        _socketClient.InteractionCreated += OnInteractionCreated;
    }

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
