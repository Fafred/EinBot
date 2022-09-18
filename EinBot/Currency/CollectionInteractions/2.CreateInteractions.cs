namespace EinBot.Currency.CollectionInteractions;

using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using EinBotDB.DataAccess;
using EinBotDB;

public partial class CollectionInteractions
{
    /// <summary>
    /// Creates a new table with the given specs.
    /// </summary>
    /// <param name="TableName">Name of the table.</param>
    /// <param name="CollectionType">Whether the table is perKey, perUser, or perRole.</param>
    /// <param name="role">The role assigned to this table.</param>
    /// <returns></returns>
    [SlashCommand("create", "Create a new currency collection.")]
    public async Task HandleCreateSlashCommand(IRole role, CollectionTypesEnum CollectionType)
    {
        try
        {
            _dataAccess.CreateTable(role.Name, CollectionType, role.Id);
            await RespondSuccessAsync($"Collection `{role.Name}` has been created and assigned to {role.Mention}.");
            return;
        } catch (InvalidNameException)
        {
            await RespondFailureAsync($"`{role.Name}` is not a valid collection name.");
            return;
        }
    }
}
