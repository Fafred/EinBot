namespace EinBot.Currency.CollectionInteractions;

using Discord.Interactions;
using EinBotDB.DataAccess;

[Group("collection", "Commands for creating a new character.  Start with /create-character start")]
public partial class CollectionInteractions : InteractionModuleBase<SocketInteractionContext>
{
    IEinDataAccess _dataAccess;

    public CollectionInteractions(IEinDataAccess einDAL)
    {
        _dataAccess = einDAL;
    }

    [SlashCommand("help", "Display informationa bout the collection command.")]
    public async Task HandleCollectionHelpCommand()
    {
        await RespondAsync("`/collection help.`");
    }

}
