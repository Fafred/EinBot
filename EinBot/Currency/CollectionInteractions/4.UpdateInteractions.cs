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
    public async Task HandleCollectionReRoleCommand(IRole OldRole, IRole NewRole)
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

    /// <summary>
    /// Instantiates a collection for a given user, key, or role.
    /// </summary>
    /// <param name="role">The role associated with the collection to instantiate.</param>
    /// <param name="user"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    [SlashCommand("instantiate", "Makes a new instance of the currency.")]
    public async Task HandleCollectionInstantiateCommand(IRole role, IUser? user = null, string? key = null)
    {
        //TODO Not allow instantiation of tables with no columns.
        var tableDefinition = _dataAccess.GetTable(role.Id);

        if (tableDefinition is null)
        {
            await RespondAsync($"There is no collection associated with {role.Mention}.");
            return;
        }

        string tableKey = "";
        string keyTypeString = "";
        string keyMention = "";

        if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerRole)
        {
            tableKey = role.Id.ToString();
            keyMention = role.Mention;
            keyTypeString = "role";
        } else if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerUser)
        {
            if (user is null)
            {
                await RespondAsync($"{role.Mention} has a collection type of `PerUser`.  You must mention a user (@UserName) in the user input in order to instantiate the collection.");
                return;
            }

            tableKey = user.Id.ToString();
            keyMention = user.Mention;
            keyTypeString = "user";
        } else if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerKey)
        {
            if (key is null)
            {
                await RespondAsync($"{role.Mention} requires a key to be provided.  Please enter a unique key (such as a character name) in the key input.");
                return;
            }

            tableKey = key;
            keyMention = key;
            keyTypeString = "key";
        } else
        {
            // TODO: log this.
            await RespondAsync($"Something has gone horribly wrong: unable to determine the collection type of {role.Mention}.");
            return;
        }

        string[] keyType = new string[] { "key", "user", "role" };
        try
        {
            _dataAccess.AddRow(tableDefinition.Id, tableKey);
        } catch (TableDoesNotExistException e)
        {
            await RespondAsync($"There is no table associated with the role {role.Mention}.");
            return;
        } catch (KeyAlreadyPresentInTableException e)
        {
            await RespondAsync($"There is already an instance of the {role.Mention} collection for the {keyTypeString} {keyMention}.");
            return;
        }

        await RespondAsync($"You have successfully instantiated a {role.Mention} collection for {keyTypeString} {keyMention}.");
        return;
    }

    /// <summary>
    /// Creating, Updating, and Deleting currencies in a collection.
    /// </summary>
    [Group("currency", "Currency related commands.")]
    public class CollectionInteractionsCurrency : InteractionModuleBase<SocketInteractionContext>
    {
        private IEinDataAccess _dataAccess;

        public CollectionInteractionsCurrency(IEinDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        /// <summary>
        /// Adds a currency of the given type to the given collection.
        /// </summary>
        /// <param name="role">The role id of the collection to add the currency to.</param>
        /// <param name="dataType">The data type of the currency to add.</param>
        /// <param name="currencyName">The name of the currency to add.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes a currency from a collection.
        /// </summary>
        /// <param name="Role">The role id of the collection to delete the currency from.</param>
        /// <param name="CurrencyName">The name of the currency to delete.</param>
        /// <param name="WriteDELETE">Must be "DELETE" in order to confirm deletion.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Renames a currency in a collection.
        /// </summary>
        /// <param name="role">The role id of the collection of the currency to rename.</param>
        /// <param name="oldCurrencyName">The current name of the currency to rename.</param>
        /// <param name="newCurrencyName">The new name to set the currency name to.</param>
        /// <returns></returns>
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