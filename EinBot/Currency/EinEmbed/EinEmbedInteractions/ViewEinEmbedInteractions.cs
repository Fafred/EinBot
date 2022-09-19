namespace EinBot.Currency.EinEmbed;

using Discord;
using Discord.Interactions;
using EinBot.Currency.EinEmbed.Extensiosn;
using EinBotDB;
using EinBotDB.DataAccess;
using EinBotDB.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class ViewEinEmbedInteractions : InteractionModuleBase<SocketInteractionContext>
{
    IEinDataAccess _dataAccess;

    public ViewEinEmbedInteractions(IEinDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    [SlashCommand("view", "Views the display created for the given currency collection.")]
    public async Task HandleViewInteraction(IRole role, string? key = null, IUser? user = null)
    {
        TableDefinitionsModel tableDefinition;
        EinTable einTable;
        EinRow einRow;

        // Make sure there's actually a table associated with the role.
        try
        {
            tableDefinition = _dataAccess.GetTable(roleId: role.Id);
            einTable = _dataAccess.GetEinTable(tableDefinition.Id);
        }
        catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"There is no collection associated with the role {role.Mention}.");
            return;
        }

        CollectionTypesEnum collectionType = (CollectionTypesEnum)tableDefinition.CollectionTypeId;

        // Make sure we have the required input.
        switch (collectionType)
        {
            case CollectionTypesEnum.PerKey:
                if (key is null)
                {
                    await RespondFailureAsync($"Collection {role.Mention} is a PerKey collection.  Must supply a key in the key input.");
                    return;
                }
                break;
            case CollectionTypesEnum.PerUser:
                if (user is null)
                {
                    await RespondFailureAsync($"Collection {role.Mention} is a PerUser collection.  Must supply a user (@User) in the user input.");
                    return;
                }
                key = user.Id.ToString();
                break;
            case CollectionTypesEnum.PerRole:
                key = role.Id.ToString();
                break;
            default:
                await RespondFailureAsync($"Unable to determine collection type of {role.Mention}.");
                return;
        }

        // See if there's a row associated with the key.
        try
        {
            einRow = einTable.GetRow(key);
        }
        catch (KeyNotFoundException)
        {
            switch (collectionType)
            {
                case CollectionTypesEnum.PerKey:
                    await RespondFailureAsync($"No instanced collection of {role.Mention} is associated with the `{key}` key.");
                    return;
                case CollectionTypesEnum.PerUser:
                    await RespondFailureAsync($"No instanced collection of {role.Mention} is associated with {user.Mention}.");
                    return;
                case CollectionTypesEnum.PerRole:
                    await RespondFailureAsync($"There is no instanced collection of {role.Mention}.");
                    return;
            }
            return;
        }

        string messageString = "_ _";
        bool isEphemeral = false;

        List<EinEmbedPartsModel>? einEmbedPartsList;

        // Now see if we have any EinEmbedParts associated with this table.
        try
        {
            einEmbedPartsList = _dataAccess.GetEmbedParts(tableId: tableDefinition.Id);
        }
        catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"There is no collection associated with the role {role.Mention}.");
            return;
        }

        if (einEmbedPartsList is null || einEmbedPartsList.Count < 1)
        {
            await RespondFailureAsync($"There is no display associated with the role {role.Mention}.");
            return;
        }

        var messagePart = einEmbedPartsList.FirstOrDefault(part => (EmbedPartsEnum)part.EmbedPartTypesId == EmbedPartsEnum.Message);

        if (messagePart is not null)
        {
            messageString = messagePart.Data01 ?? "_ _";
            einEmbedPartsList.Remove(messagePart);
        }

        var ephemeralPart = einEmbedPartsList.FirstOrDefault(part => (EmbedPartsEnum)part.EmbedPartTypesId == EmbedPartsEnum.Ephemeral);
        if (ephemeralPart is not null)
        {
            bool.TryParse(ephemeralPart.Data01, out isEphemeral);
            einEmbedPartsList.Remove(ephemeralPart);
        }

        var embedBuilder = new EmbedBuilder();

        foreach (var einEmbedPart in einEmbedPartsList)
        {
            embedBuilder.AddEinEmbedPart(einRow, einEmbedPart);
        }

        await RespondAsync(messageString, embed: embedBuilder.Build(), ephemeral: isEphemeral);
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
