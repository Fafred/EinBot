namespace EinBot.Currency.CollectionInteractions;

using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using EinBotDB.DataAccess;
using EinBotDB;
using EinBotDB.Models;
using System.Data;

public partial class CollectionInteractions
{
    /// <summary>
    /// Assigns a new role to the collection.
    /// </summary>
    /// <param name="OldRole">The old role assigned to the collection.</param>
    /// <param name="NewRole">The role to replace the old role.</param>
    /// <returns></returns>
    [SlashCommand("re-role", "Assigns a different role to the collection")]
    public async Task HandleCollectionAssignRoleCommand(IRole OldRole, IRole NewRole)
    {
        try
        {
            _dataAccess.RenameTable(OldRole.Id, NewRole.Name);
            _dataAccess.SetTableRole(OldRole.Id, NewRole.Id);
        } catch (InvalidNameException e)
        {
            await RespondAsync($"{NewRole.Name} is not a valid table name.");
            return;
        } catch (TableDoesNotExistException e)
        {
            await RespondAsync($"There is no currency associated with {OldRole.Mention}.");
            return;
        }

        await RespondAsync($"Collection {OldRole.Mention} has been reassigned to role {NewRole.Mention}.");
        return;

    }

    [Group("currency", "Currency related commands.")]
    public class CollectionInteractionsCurrency : InteractionModuleBase<SocketInteractionContext>
    {
        private IEinDataAccess _dataAccess;

        public CollectionInteractionsCurrency(IEinDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [SlashCommand("add", "Add a currency. /collection currency add {Role} {DataType} {Currency Name}")]
        public async Task HandleCollectionCurrencyAddCommand(IRole role, DataTypesEnum dataType, string currencyName)
        {
            try
            {
                _dataAccess.CreateColumn(role.Id, currencyName, dataType);
            } catch (TableDoesNotExistException e)
            {
                await RespondAsync($"There is no collection associated with {role.Mention}.");
                return;
            } catch (InvalidNameException e)
            {
                await RespondAsync($"The name \"`{currencyName}`\" is invalid.  Please choose a name with only alpha-numeric characters for the currency.");
                return;
            } catch (ColumnAlreadyExistsException e)
            {
                await RespondAsync($"There is already a currency with the name \"`{currencyName}'\" in the {role.Mention} collection.  Please choose a different currency name.");
                return;
            }

            await RespondAsync($"The currency \"'{currencyName}'\" has been added to collection {role.Mention}.");
            return;
        }

        [SlashCommand("delete", "Deletes a currency. /collection currency delete {Role} {Currency Name} DELETE")]
        public async Task HandleCollectionCurrencyDeleteCommand(IRole Role, string CurrencyName, string? WriteDELETE = null)
        {
            if (string.IsNullOrEmpty(WriteDELETE) || !WriteDELETE.Equals("DELETE"))
            {
                await RespondAsync("You must type `DELETE` after the role and currency name in order to confirm deletion.");
                return;
            }

            try
            {
                _dataAccess.DeleteColumn(Role.Id, CurrencyName);
                await RespondAsync($"`{CurrencyName}` has been removed from the {Role.Mention} collection.");
            } catch (TableDoesNotExistException e)
            {
                await RespondAsync($"There is no collection assigned to {Role.Mention}.");
                return;
            } catch (ColumnDoesNotExistException e)
            {
                await RespondAsync($"There is no currency named `{CurrencyName}` in the {Role.Mention} collection.  Currencies are CASE-SENSITIVE.");
                return;
            }
        }

        [SlashCommand("rename", "Renames a currency. /collection currency {Role} {Old Currency Name} {New Currency Name}")]
        public async Task HandleCollectionCurrencyRenameCommand(IRole role, string oldCurrencyName, string newCurrencyName)
        {
            try
            {
                _dataAccess.RenameColumn(role.Id, oldCurrencyName, newCurrencyName);
                await RespondAsync($"The currency `{oldCurrencyName}` has been renamed to `{newCurrencyName}` in the collection {role.Mention}.");
                return;
            }
            catch (TableDoesNotExistException e)
            {
                await RespondAsync($"There is no collection assigned to {role.Mention}.");
                return;
            }
            catch (ColumnDoesNotExistException e)
            {
                await RespondAsync($"There is no currency named `{oldCurrencyName}` in the {role.Mention} collection.  Currencies are CASE-SENSITIVE.");
                return;
            }
        }
    }
}