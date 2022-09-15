namespace EinBot.Currency.EinEmbed;

using Discord;
using Discord.Interactions;
using EinBotDB;
using EinBotDB.DataAccess;
using EinBotDB.Models;
using System.Text;

[Group("build", "Commands for building custom embed displays for roles.")]
public partial class BuildEinEmbedInteractions : InteractionModuleBase<SocketInteractionContext>
{
    IEinDataAccess _dataAccess;

    public BuildEinEmbedInteractions(IEinDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    [SlashCommand("info", "Displays info about a custom display for a collection.")]
    public async Task HandleBuildInfoCommand(IRole role)
    {
        Embed embed = GetInfoDisplay(role).Result;

        if (embed is null)
        {
            await RespondFailureAsync($"There is no collection associated with the role {role.Mention}.");
            return;
        }

        await RespondAsync(embed: embed);
    }

    [SlashCommand("sequence", "Changes the sequence of a part in the display for the given role.")]
    public async Task HandleBuildSequenceCommand(IRole role, int partId, int newSequenceNumber)
    {
        TableDefinitionsModel tableDefinition;
        try
        {
            tableDefinition = _dataAccess.GetTable(roleId: role.Id);
        } catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"There is no collection associated with {role.Mention}.  Use `/collection list` to see available collections.");
            return;
        }

        try
        {
            var embedPart = _dataAccess.GetEmbedPart(partId, roleId: role.Id);

            if (embedPart is null)
            {
                await RespondFailureAsync($"The {role.Mention} collection does not have a display part with id {partId}.");
                return;
            }

            _dataAccess.SetEmbedPartSequence(partId, newSequenceNumber);
        } catch (EinEmbedPartDoesNotExistException)
        {
            await RespondFailureAsync($"There is no embed part with the id {partId}.  Use `/build info` to look up the embed part id.");
            return;
        }

        await RespondSuccessAsync($"Moved the embed part with id {partId} to sequence # {newSequenceNumber}.");
    }

    private async Task<Embed> GetInfoDisplay(IRole role)
    {

        var tableDefinition = _dataAccess.GetTable(role.Id);

        if (tableDefinition is null)
        {
            return null;
        }

        List<EinEmbedPartsModel>? embedParts = _dataAccess.GetEmbedParts(roleId: role.Id);

        EmbedBuilder embedBuilder = new EmbedBuilder();

        embedBuilder.WithTitle($"Info Display for {role.Name}.");
        embedBuilder.WithColor(role.Color);

        if (embedParts is null || embedParts.Count < 1)
        {
            embedBuilder.WithDescription("There is currently no display created for this collection.\n\nUse the /build add command to create a display.");
            return embedBuilder.Build();
        }

        embedBuilder.WithDescription("The following items are the parts of the custom embed.\n_ _\nThe `[id]` of the item is used to edit it with `/build remove`, `/build data`, or `/build sequence`.\n_ _\nThe `[#]` is the order in which the item is added.  This only matters for fields.  You can change the order using the item's id with `/build sequence`.\n_ _\n:small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: \n_ _");

        StringBuilder sb = new StringBuilder();
        foreach(var part in embedParts)
        {
            string title = $"[id: {part.Id}] [#{part.Sequence:D2}]\t{(EmbedPartsEnum)part.EmbedPartTypesId}";

            if (!string.IsNullOrEmpty(part.Data01)) sb.AppendLine($"........**[1]**: {part.Data01}");
            if (!string.IsNullOrEmpty(part.Data02)) sb.AppendLine($"........**[2]**: {part.Data02}");
            if (!string.IsNullOrEmpty(part.Data03)) sb.AppendLine($"........**[3]**: {part.Data03}");
            sb.Append("\n:small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: :small_blue_diamond: \n_ _");

            embedBuilder.AddField(title, sb.ToString());

            sb.Clear();
        }

        return embedBuilder.Build();
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
