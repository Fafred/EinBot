namespace EinBot.Currency.CollectionInteractions;

using Discord;
using Discord.Interactions;
using EinBotDB;
using System.Threading.Tasks;

public partial class CollectionInteractions
{
    /// <summary>
    /// Deletes a table through the Discord API.
    /// </summary>
    /// <param name="Role">The role associated with the table.</param>
    /// <param name="WriteDELETE">This must be a string equal to "DELETE" in order for the delete to take effect.</param>
    /// <returns></returns>
    [SlashCommand("delete", "Delete a table.  Usage: /collection delete {@Role} DELETE")]
    public async Task HandleCollectionDeleteCommand(IRole Role, string? WriteDELETE = null)
    {
        if (string.IsNullOrEmpty(WriteDELETE) || !WriteDELETE.ToLower().Trim().Equals("delete"))
        {
            await RespondFailureAsync("You must write `DELETE` after the role.");
            return;
        }

        try
        {
            var table = _dataAccess.DeleteTable(roleId: Role.Id);

            await RespondAsync($"`[Success]` You have deleted the `{table.Name}` collection associated with the role {Role.Mention}.");
            return;
        }
        catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"There is no table associated with {Role.Mention}.");
            return;
        }
    }
}