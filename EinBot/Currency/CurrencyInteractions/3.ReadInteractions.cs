namespace EinBot.Currency.CurrencyInteractions;

using Discord;
using Discord.Interactions;
using EinBot.Currency.CurrencyInteractions.Exceptions;
using EinBotDB;
using EinBotDB.DataAccess;
using EinBotDB.Models;

public partial class CurrencyInteractions
{
    [SlashCommand("view", "Displays the value of the currency.")]
    public async Task HandleCurrencyViewCommand(IRole role, string currencyName, string? currencyKey = null, IUser? user = null)
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

        var value = _dataAccess.GetCellValue(tableId: tableId, columnId: columnId, rowKey: key);

        await RespondSuccessAsync($"The value of {currencyName} in collection {role.Mention} for {collectionType} {keyMention} is ` {value} `.");
    }
}
