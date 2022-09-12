namespace EinBot.Currency.CollectionInteractions;

using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using EinBotDB.DataAccess;
using EinBotDB;
using EinBot.Currency.Extensions;

public partial class CollectionInteractions
{
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

                embedDisplay.AddField($"{role.Mention} - {(CollectionTypesEnum)tableDefinition.CollectionTypeId}", "_ _", inline: false);
            } catch (TableDoesNotExistException e)
            {
                continue;
            }
        }

        await RespondAsync(embed: embedDisplay.Build());
        return;
    }
}