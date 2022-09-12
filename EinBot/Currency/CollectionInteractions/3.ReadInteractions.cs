namespace EinBot.Currency.CollectionInteractions;

using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using EinBotDB.DataAccess;
using EinBotDB;
using EinBot.Currency.Extensions;

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
            einTable = _dataAccess.GetEinTable(Role.Id);
        } catch (TableDoesNotExistException e)
        {
            await RespondAsync($"There is no collection associated with the role {Role.Mention}.");
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

        foreach(IRole role in guildRolesList)
        {
            try
            {
                var tableDefinition = _dataAccess.GetTable(role.Id);

                embedDisplay.AddField(
                    $"{role.Mention}",
                    $"*Type*: {(CollectionTypesEnum)tableDefinition.CollectionTypeId}\n*Id*: {tableDefinition.Id}\n*Name*: {tableDefinition.Name}\n*Currencies*: {tableDefinition.ColumnDefinitions.Count}",
                    inline: false);

            } catch (TableDoesNotExistException e)
            {
                continue;
            }
        }

        await RespondAsync(embed: embedDisplay.Build());
        return;
    }
}