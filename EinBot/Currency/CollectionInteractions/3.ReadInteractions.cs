namespace EinBot.Currency.CollectionInteractions;

using Discord;
using Discord.Interactions;
using EinBot.Currency.Extensions;
using EinBotDB;
using EinBotDB.DataAccess;
using System.Threading.Tasks;

public partial class CollectionInteractions
{
    /// <summary>
    /// Displays the collection and the currencies tied to it.
    /// </summary>
    /// <param name="Role">The role which has been assigned to the collection.</param>
    /// <returns></returns>
    [SlashCommand("info", "Usage /collection view {@Role}")]
    public async Task HandleCollectionViewCommand(IRole Role)
    {
        EinTable einTable;

        try
        {
            einTable = _dataAccess.GetEinTable(roleId: Role.Id);
        }
        catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"There is no collection associated with the role {Role.Mention}.");
            return;
        }

        Embed embed = new EmbedBuilder().DisplayEinTable(einTable, Role).Build();

        await RespondAsync(embed: embed);
    }

    /// <summary>
    /// Displays a list of the collections available on the caller's Discord guild.
    /// </summary>
    /// <returns></returns>
    [SlashCommand("list", "Lists all the availble collections.")]
    public async Task HandleCollectionListCommand()
    {
        var embedDisplay = new EmbedBuilder()
            .WithTitle("Collections")
            .WithColor(Color.Purple)
            .WithDescription("The following collections are available:");

        var guildRolesList = Context.Guild.Roles;

        foreach (IRole role in guildRolesList)
        {
            try
            {
                var einTable = _dataAccess.GetEinTable(roleId: role.Id);
                var collectionType = einTable.CollectionTypeName;
                var tableId = einTable.TableId;
                var currencyCount = einTable.ColumnDataTypes.Count;
                var instances = einTable.Rows.Count;

                embedDisplay.AddField(
                    "_ _",
                    $"**{role.Mention}**\n*Type*: {collectionType}\n*Currencies*: {currencyCount}\n*Instances*: {instances}",
                    inline: true);

            }
            catch (TableDoesNotExistException)
            {
                continue;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
            }
        }

        await RespondAsync(embed: embedDisplay.Build());
        return;
    }
}