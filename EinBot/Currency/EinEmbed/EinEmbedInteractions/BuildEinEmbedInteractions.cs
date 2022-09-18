namespace EinBot.Currency.EinEmbed;

using Discord;
using Discord.Interactions;
using EinBotDB;
using EinBotDB.DataAccess;
using EinBotDB.Models;
using System.Text;

[Group("display", "Commands for building custom embed displays for roles.")]
public partial class BuildEinEmbedInteractions : InteractionModuleBase<SocketInteractionContext>
{
    IEinDataAccess _dataAccess;

    public BuildEinEmbedInteractions(IEinDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    [SlashCommand("add", "Adds a part to the display of the given collection.")]
    public async Task HandleBuildAddCommand(IRole role, EmbedPartsEnum partType, string data01, string? data02 = null, string? data03 = null)
    {
        if (string.IsNullOrEmpty(data01))
        {
            await RespondFailureAsync("You must include data in at least the first data slot.");
            return;
        }

        TableDefinitionsModel tableDefinition;

        try
        {
            tableDefinition = _dataAccess.GetTable(roleId: role.Id);
        }
        catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"There is no collection associated with the role {role.Mention}.");
            return;
        }

        var partId = _dataAccess.AddEmbedPart(partType, data01, Data02: data02, Data03: data03, tableId: tableDefinition.Id);

        await RespondSuccessAsync($"You have successfully added {partType}[id: {partId}] to the display of {role.Mention}.");
    }

    [SlashCommand("data", "Sets the data fields for one of the embed parts.")]
    public async Task HandleBuildDataCommand(IRole role, int partId, string? data01 = null, string? data02 = null, string? data03 = null)
    {
        if (data01 is null && data02 is null && data03 is null)
        {
            await RespondFailureAsync($"No new data provided. No changes were made.");
            return;
        }

        TableDefinitionsModel tableDefinition;

        try
        {
            tableDefinition = _dataAccess.GetTable(roleId: role.Id);
        }
        catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"There is no collection associated with {role.Mention}.");
            return;
        }

        EinEmbedPartsModel? embedPart;

        try
        {
            embedPart = _dataAccess.GetEmbedPart(partId, roleId: role.Id);
        }
        catch (EinEmbedPartDoesNotExistException)
        {
            await RespondFailureAsync($"There is no embed part with id {partId} in the {role.Mention} collection.");
            return;
        }

        StringBuilder stringBuilder = new StringBuilder();
        if (data01 is not null)
        {
            if (data01.Equals("{DELETE}"))
            {
                data01 = "";
            }

            _dataAccess.SetEmbedPartData(embedPart!.Id, 1, data01);

            if (string.IsNullOrEmpty(data01)) data01 = "[NULL]";

            stringBuilder.Append($"\n........Data 1 set to `{data01}`.");
        }
        if (data02 is not null)
        {
            if (data02.Equals("{DELETE}"))
            {
                data02 = "";
            }

            _dataAccess.SetEmbedPartData(embedPart!.Id, 2, data02);

            if (string.IsNullOrEmpty(data02)) data02 = "[NULL]";

            stringBuilder.Append($"\n........Data 2 set to `{data02}`.");
        }
        if (data03 is not null)
        {
            if (data03.Equals("{DELETE}"))
            {
                data03 = "";
            }

            _dataAccess.SetEmbedPartData(embedPart!.Id, 3, data03);

            if (string.IsNullOrEmpty(data03)) data03 = "[NULL]";

            stringBuilder.Append($"\n........Data 3 set to `{data03}`.");
        }

        await RespondSuccessAsync($"You have made the following changes to the {role.Mention} display part {(EmbedPartsEnum)embedPart!.EmbedPartTypesId}[id {partId}]:{stringBuilder.ToString()}");
        return;
    }

    [SlashCommand("delete", "Deletes a collection's display.  Must write DELETE in the write-delete prompt to confirm.")]
    public async Task HandleBuildDeleteCommand(IRole role, string? writeDelete = null)
    {
        if (string.IsNullOrEmpty(writeDelete) || !writeDelete!.Equals("DELETE"))
        {
            await RespondFailureAsync($"You must write `DELETE` in the proper prompt to confirm deletion.");
            return;
        }

        try
        {
            _dataAccess.RemoveAllEmbedParts(roleId: role.Id);
        }
        catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"There is no collection associated with the role {role.Mention}.");
            return;
        }

        await RespondSuccessAsync($"Any existing display parts for collection {role.Mention} have been removed.");
    }

    [SlashCommand("info", "Displays info about a custom display for a collection.")]
    public async Task HandleBuildInfoCommand(IRole role)
    {
        Embed? embed = GetInfoDisplay(role);

        if (embed is null)
        {
            await RespondFailureAsync($"There is no collection associated with the role {role.Mention}.");
            return;
        }

        await RespondAsync(embed: embed);
    }

    [SlashCommand("remove", "Removes a display part from a collection display.")]
    public async Task HandleBuildRemoveCommand(IRole role, int partId, string? writeDelete = null)
    {
        if (string.IsNullOrEmpty(writeDelete) || !writeDelete!.Equals("DELETE"))
        {
            await RespondFailureAsync($"You must write `DELETE` in the proper prompt to confirm deletion.");
            return;
        }

        TableDefinitionsModel tableDefinition;

        try
        {
            tableDefinition = _dataAccess.GetTable(roleId: role.Id);
        }
        catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"No collection is associated with {role.Mention}.");
            return;
        }

        EinEmbedPartsModel einEmbedPart;

        try
        {
            einEmbedPart = _dataAccess.GetEmbedPart(partId, tableId: tableDefinition.Id)!;
        }
        catch (EinEmbedPartDoesNotExistException)
        {
            await RespondFailureAsync($"{role.Mention} collection display does not have a display part with id `{partId}`.");
            return;
        }

        _dataAccess.RemoveEmbedPart(einEmbedPart!.Id);
        await RespondSuccessAsync($"You have removed {(EmbedPartsEnum)einEmbedPart.EmbedPartTypesId}[id: {partId}] from the display of {role.Mention}.");
    }

    [SlashCommand("sequence", "Changes the sequence of a part in the display for the given role.")]
    public async Task HandleBuildSequenceCommand(IRole role, int partId, int newSequenceNumber)
    {
        TableDefinitionsModel tableDefinition;
        try
        {
            tableDefinition = _dataAccess.GetTable(roleId: role.Id);
        }
        catch (TableDoesNotExistException)
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
        }
        catch (EinEmbedPartDoesNotExistException)
        {
            await RespondFailureAsync($"There is no embed part with the id {partId}.  Use `/build info` to look up the embed part id.");
            return;
        }

        await RespondSuccessAsync($"Moved the embed part with id {partId} to sequence # {newSequenceNumber}.");
    }

    private Embed? GetInfoDisplay(IRole role)
    {

        var tableDefinition = _dataAccess.GetTable(roleId: role.Id);

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
        foreach (var part in embedParts)
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
