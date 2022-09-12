namespace EinBot.Currency.CollectionInteractions;

using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using EinBotDB.DataAccess;
using EinBotDB;

public partial class CollectionInteractions
{
    [SlashCommand("delete", "Delete a table.  Usage: /collection delete {@Role} DELETE")]
    public async Task HandleCollectionDeleteCommand(IRole Role, string? WriteDELETE = null)
    {
        if (string.IsNullOrEmpty(WriteDELETE) || !WriteDELETE.ToLower().Trim().Equals("delete"))
        {
            await RespondAsync("You must write `DELETE` after the role.");
            return;
        }

        try
        {
            var table = _dataAccess.DeleteTable(Role.Id);

            await RespondAsync($"You have deleted the `{table.Name}` collection associated with the role {Role.Mention}.");
            return;
        } catch (TableDoesNotExistException e)
        {
            await RespondAsync($"There is no table associated with {Role.Mention}.");
            return;
        }
    }
}