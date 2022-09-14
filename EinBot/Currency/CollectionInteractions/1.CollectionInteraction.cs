namespace EinBot.Currency.CollectionInteractions;

using Discord.Interactions;
using EinBotDB.DataAccess;


/// <summary>
/// This class handles the creation of collections and currencies in them through the Discord API.
/// </summary>
[Group("collection", "Commands for creating, reading, updating, and deleting collections of currencies.")]
public partial class CollectionInteractions : InteractionModuleBase<SocketInteractionContext>
{
    IEinDataAccess _dataAccess;

    public CollectionInteractions(IEinDataAccess einDAL)
    {
        _dataAccess = einDAL;
    }

    /// <summary>
    /// A help command, which details the usage of commands.
    /// </summary>
    /// <returns></returns>
    [SlashCommand("help", "Display informationa bout the collection command.")]
    public async Task HandleCollectionHelpCommand()
    {
        await RespondAsync("Not yet implemented.");
    }

    private async Task RespondSuccessAsync(string message, bool ephemeral = false)
    {
        await RespondAsync($"```diff\n+[Success]+\n```\n{message}", ephemeral: ephemeral);
    }

    private async Task RespondFailureAsync(string message, bool ephemeral = false)
    {
        await RespondAsync($"```diff\n-[Failure]-\n```\n{message}", ephemeral: ephemeral);
    }
}
