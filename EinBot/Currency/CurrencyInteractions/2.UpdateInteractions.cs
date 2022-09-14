namespace EinBot.Currency.CurrencyInteractions;

using Discord;
using Discord.Interactions;
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
        string collectionType;

        try
        {
            ValidateInput(role, currencyName, 
                out tableDefinition, out columnDefinition,
                out key, out collectionType,
                user: user, collectionKey: currencyKey);
        } catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"No table is associated with the role {role.Mention}.");
            return;
        } catch (ColumnDoesNotExistException)
        {
            await RespondFailureAsync($"{role.Mention} does not contain a currency named `{currencyName}`.");
            return;
        } catch (CollectionTypeIsPerKeyException)
        {
            await RespondFailureAsync($"{role.Mention} is a PerKey type collection.  Please enter in a key in the key input.");
            return;
        } catch (CollectionTypeIsPerUserException)
        {
            await RespondFailureAsync($"{role.Mention} is a PerUser type collection.  Please enter in a user (@User) in the user input.");
            return;
        }

        // Attempt to set the value.
        try
        {
            _dataAccess.SetCellValue(tableDefinition!.Id, columnDefinition!.Name, setValue, rowKey: key);
        } catch (InvalidDataException e)
        {
            await RespondFailureAsync($"{role.Mention}'s {e.Message}.");
            return;
        } catch (TableDoesNotExistException)
        {
            await RespondFailureAsync($"No table is associated with the role {role.Mention}.");
            return;
        }   catch (ColumnDoesNotExistException)
        {
            await RespondFailureAsync($"{role.Mention} does not contain a currency named `{currencyName}`.");
            return;
        } catch (CellDoesNotExistException)
        {
            await RespondFailureAsync($"Unable to find the cell for {role.Mention}'s `{currencyName}`.");
            return;
        }

        await RespondSuccessAsync($"{role.Mention} {currencyName} has been set to `{setValue}` for {collectionType} {key}.");
    }

    [SlashCommand("modify", "Modifies the value of a currency.")]
    public async Task HandleCurrencyEditCommand(IRole role, string currencyName, string modifyValue, IUser? user = null, string? currencyKey = null)
    {
        TableDefinitionsModel? tableDefinition;
        ColumnDefinitionsModel? columnDefinition;
        string key;
        string collectionType;

        try
        {
            ValidateInput(role, currencyName,
                out tableDefinition, out columnDefinition,
                out key, out collectionType,
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

        // Attempt to set the value.
        try
        {
            _dataAccess.ModifyCellValue(tableDefinition!.Id, columnDefinition!.Name, modifyValue, rowKey: key);
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

        await RespondSuccessAsync($"{role.Mention} {currencyName} has been modified by `{modifyValue}` for {collectionType} {key}.");
    }

    private void ValidateInput(IRole role, string currencyName,
                            out TableDefinitionsModel? tableDefinition, out ColumnDefinitionsModel? columnDefinition,
                            out string key, out string collectionType,
                             IUser? user = null, string? collectionKey = null)
    {
        tableDefinition = _dataAccess.GetTable(role.Id);

        // Make sure the table exists.
        if (tableDefinition is null) throw new TableDoesNotExistException();

        // Make sure the column exists.
        columnDefinition = _dataAccess.GetColumns(tableDefinition.Id)
                                    .FirstOrDefault(column => column.Name.Equals(currencyName));


        if (columnDefinition is null) throw new ColumnDoesNotExistException("");

        key = collectionKey ?? "";
        collectionType = "key";
        if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerRole)
        {
            key = role.Id.ToString();
            collectionType = "role";
        }
        else if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerUser)
        {
            if (user is null) throw new CollectionTypeIsPerUserException();

            key = user.Id.ToString();
            collectionType = "user";
        }
        else if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerKey)
        {
            if (string.IsNullOrEmpty(key)) throw new CollectionTypeIsPerKeyException();
        }
    }
}
