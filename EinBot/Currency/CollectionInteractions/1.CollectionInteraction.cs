namespace EinBot.Currency.CollectionInteractions;

using Discord.Interactions;
using EinBotDB.DataAccess;


/// <summary>
/// This class handles the creation of collections and currencies in them through the Discord API.
/// </summary>
[Group("collection", "Commands for creating a new character.  Start with /create-character start")]
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
        await RespondAsync("`/collection help.`");
    }

}
