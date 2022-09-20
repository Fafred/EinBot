namespace EinBot.Currency.CurrencyInteractions;

using Discord;
using Discord.Interactions;
using EinBot.Currency.CurrencyInteractions.Exceptions;
using EinBotDB;
using EinBotDB.DataAccess;
using EinBotDB.Models;
using System.Text;

public partial class CurrencyInteractions
{
    [SlashCommand("set", "Sets the value of a currency.")]
    public async Task HandleCurrencySetCommand(IRole role, string currencyName, string setValue, IUser? user = null, string? currencyKey = null)
    {
        TableDefinitionsModel? tableDefinition;
        ColumnDefinitionsModel? columnDefinition;
        string key;
        string keyMention;
        string collectionType;

        try
        {
            ValidateInput(role, currencyName,
                out tableDefinition, out columnDefinition,
                out key, out collectionType, out keyMention,
                user: user, collectionKey: currencyKey);
        }
        catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"No table is associated with the role {role.Mention}.");
            return;
        }
        catch (ColumnDoesNotExistException)
        {
            await RespondFailureAsync($"{role.Mention} does not contain a currency named `{currencyName}`.");
            return;
        }
        catch (CollectionTypeIsPerKeyException)
        {
            await RespondFailureAsync($"{role.Mention} is a PerKey type collection.  Please enter in a key in the key input.");
            return;
        }
        catch (CollectionTypeIsPerUserException)
        {
            await RespondFailureAsync($"{role.Mention} is a PerUser type collection.  Please enter in a user (@User) in the user input.");
            return;
        }

        var tableId = tableDefinition!.Id;
        var columnId = columnDefinition!.Id;
        // Attempt to set the value.
        try
        {
            _dataAccess.SetCellValue(setValue, tableId: tableId, columnId: columnId, rowKey: key);
        }
        catch (InvalidDataException e)
        {
            await RespondFailureAsync($"{role.Mention}'s {e.Message}.");
            return;
        }
        catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"No table is associated with the role {role.Mention}.");
            return;
        }
        catch (ColumnDoesNotExistException)
        {
            await RespondFailureAsync($"{role.Mention} does not contain a currency named `{currencyName}`.");
            return;
        }
        catch (CellDoesNotExistException)
        {
            await RespondFailureAsync($"Unable to find the cell for {role.Mention}'s `{currencyName}`.");
            return;
        }

        await RespondSuccessAsync($"{role.Mention} {currencyName} has been set to `{setValue}` for {collectionType} {keyMention}.");
    }

    [SlashCommand("modify", "Modifies the value of a currency.")]
    public async Task HandleCurrencyEditCommand(IRole role, string currencyName, string modifyValue, IUser? user = null, string? currencyKey = null)
    {
        TableDefinitionsModel? tableDefinition;
        ColumnDefinitionsModel? columnDefinition;
        string key;
        string keyMention;
        string collectionType;

        try
        {
            ValidateInput(role, currencyName,
                out tableDefinition, out columnDefinition,
                out key, out collectionType, out keyMention,
                user: user, collectionKey: currencyKey);
        }
        catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"No table is associated with the role {role.Mention}.");
            return;
        }
        catch (ColumnDoesNotExistException)
        {
            await RespondFailureAsync($"{role.Mention} does not contain a currency named `{currencyName}`.");
            return;
        }
        catch (CollectionTypeIsPerKeyException)
        {
            await RespondFailureAsync($"{role.Mention} is a PerKey type collection.  Please enter in a key in the key input.");
            return;
        }
        catch (CollectionTypeIsPerUserException)
        {
            await RespondFailureAsync($"{role.Mention} is a PerUser type collection.  Please enter in a user (@User) in the user input.");
            return;
        }

        int tableId = tableDefinition!.Id;
        int columnId = columnDefinition!.Id;
        string? oldValue;
        string? newValue;
        // Attempt to set the value.
        try
        {
            oldValue = _dataAccess.GetCellValue(tableId: tableId, columnId: columnId, rowKey: key);

            oldValue ??= "[NO VALUE]";

            _dataAccess.ModifyCellValue(modifyValue, tableId: tableId, columnId: columnId, rowKey: key);

            newValue = _dataAccess.GetCellValue(tableId: tableId, columnId: columnId, rowKey: key);
        }
        catch (InvalidDataException e)
        {
            await RespondFailureAsync($"{role.Mention}'s {e.Message}.");
            return;
        }
        catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"No table is associated with the role {role.Mention}.");
            return;
        }
        catch (ColumnDoesNotExistException)
        {
            await RespondFailureAsync($"{role.Mention} does not contain a currency named `{currencyName}`.");
            return;
        }
        catch (CellDoesNotExistException)
        {
            await RespondFailureAsync($"Unable to find the cell for {role.Mention}'s `{currencyName}`.");
            return;
        }

        await RespondSuccessAsync($"{role.Mention} {currencyName} for {collectionType} {keyMention} has been modified by `{modifyValue}`.  It went from `{oldValue}` to `{newValue}`.");
    }

    private void ValidateInput(IRole role, string currencyName,
                            out TableDefinitionsModel? tableDefinition, out ColumnDefinitionsModel? columnDefinition,
                            out string key, out string collectionType,
                            out string keyMention,
                             IUser? user = null, string? collectionKey = null)
    {
        tableDefinition = _dataAccess.GetTable(roleId: role.Id);

        // Make sure the table exists.
        if (tableDefinition is null) throw new TableDoesNotExistException();

        // Make sure the column exists.
        columnDefinition = _dataAccess.GetColumn(columnName: currencyName, tableId: tableDefinition.Id);

        if (columnDefinition is null) throw new ColumnDoesNotExistException("");

        key = collectionKey ?? "";
        keyMention = key;
        collectionType = "key";

        if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerRole)
        {
            key = role.Id.ToString();
            keyMention = role.Mention;
            collectionType = "role";
        }
        else if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerUser)
        {
            if (user is null) throw new CollectionTypeIsPerUserException();

            key = user.Id.ToString();
            keyMention = user.Mention;
            collectionType = "user";
        }
        else if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerKey)
        {
            if (string.IsNullOrEmpty(key)) throw new CollectionTypeIsPerKeyException();
        }
    }
}
