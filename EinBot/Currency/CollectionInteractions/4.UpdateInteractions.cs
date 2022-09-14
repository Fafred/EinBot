namespace EinBot.Currency.CollectionInteractions;

using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using EinBotDB.DataAccess;
using EinBotDB;
using EinBotDB.Models;
using System.Data;
using Microsoft.EntityFrameworkCore.Query;

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
            await RespondFailureAsync($"{NewRole.Name} is not a valid table name.");
            return;
        } catch (TableDoesNotExistException e)
        {
            await RespondFailureAsync($"There is no currency associated with {OldRole.Mention}.");
            return;
        }

        await RespondSuccessAsync($"Collection {OldRole.Mention} has been reassigned to role {NewRole.Mention}.");
        return;

    }

    /// <summary>
    /// Uninstantiates a particular instance of a collection via Discord command.
    /// If both user and key are null, then it assumes the collection is type PerRole.
    /// </summary>
    /// <param name="role">The role associated with the collection.</param>
    /// <param name="user">If the collection is PerUser, the user associated with the collection instance.  Otherwise null.</param>
    /// <param name="key">If the collection is PerKey, the key associated with the collection instance.  Otherwise null.</param>
    /// <returns></returns>
    [SlashCommand("uninstantiate", "Removes an instance of the currency.")]
    public async Task HandleCollectionUninstantiateCommand(IRole role, IUser? user = null, string? key = null)
    {
        //Make sure the table actually exists.
        var tableDefinition = _dataAccess.GetTable(role.Id);

        if (tableDefinition is null)
        {
            await RespondFailureAsync($"There is no collection associated with {role.Mention}.");
            return;
        }

        // Validate input.
        if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerUser
            && user is null)
        {
            await RespondFailureAsync($"{role.Mention} is a PerUser collection.  Must supply a user (@User) in the user input in order to uninstantiate a collection instance.");
            return;
        } else if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerKey
                    && string.IsNullOrEmpty(key))
        {
            await RespondFailureAsync($"{role.Mention} is a PerKey collection.  Must supply a key in the key input in order to uninstantiate a collection instance.");
            return;
        }

        string keyType = "key";
        string keyMention = key;

        // Set the key.
        if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerUser)
        {
            key = user!.Id.ToString();
            keyMention = user!.Mention;
            keyType = "user";
        } else if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerRole)
        {
            key = role.Id.ToString();
            keyMention = role.Mention;
            keyType = "role";
        }

        try
        {
            _dataAccess.DeleteRow(tableDefinition.Id, key: key);
        } catch (TableDoesNotExistException e)
        {
            // Probably could never get here, but just in case between the entrance and now someone has deleted the table.
            await RespondFailureAsync($"There is no table associated with the role {role.Mention}.");
            return;
        } catch (InvalidKeyException) {
            await RespondFailureAsync($"{role.Mention} does not have an instanced collection assigned to the {keyType} {keyMention}.");
            return;
        }

        await RespondSuccessAsync($"The {keyType} `{keyMention}` instance of {role.Mention} has been uninstantiated.");
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
        //Make sure the table actually exists.
        var tableDefinition = _dataAccess.GetTable(role.Id);

        if (tableDefinition is null)
        {
            await RespondFailureAsync($"There is no collection associated with {role.Mention}.");
            return;
        }

        // Don't allow instantiation of tables with no columns.
        var columnDefinitions = _dataAccess.GetColumns(tableDefinition.Id);

        if (columnDefinitions is null || columnDefinitions.Count < 1)
        {
            await RespondFailureAsync($"Collection {role.Mention} has no currencies; cannot instantiate until it has at least one currency.");
            return;
        }

        string tableKey = "";
        string keyTypeString = "";
        string keyMention = "";

        // Validate input.
        //  If the collection type is per role: don't require user or key input.
        //  If the collection type is per user: require user input
        //  If the collection type is per key: require key input.
        if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerRole)
        {
            tableKey = role.Id.ToString();
            keyMention = role.Mention;
            keyTypeString = "role";
        } else if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerUser)
        {
            if (user is null)
            {
                await RespondFailureAsync($"{role.Mention} has a collection type of `PerUser`.  You must mention a user (@UserName) in the user input in order to instantiate the collection.");
                return;
            }

            tableKey = user.Id.ToString();
            keyMention = user.Mention;
            keyTypeString = "user";
        } else if (tableDefinition.CollectionTypeId == (int)CollectionTypesEnum.PerKey)
        {
            if (key is null)
            {
                await RespondFailureAsync($"{role.Mention} requires a key to be provided.  Please enter a unique key (such as a character name) in the key input.");
                return;
            }

            tableKey = key;
            keyMention = key;
            keyTypeString = "key";
        } else
        {
            // TODO: log this.
            await RespondFailureAsync($"Something has gone horribly wrong: unable to determine the collection type of {role.Mention}.");
            return;
        }

        string[] keyType = new string[] { "key", "user", "role" };
        try
        {
            _dataAccess.AddRow(tableDefinition.Id, tableKey);
        } catch (TableDoesNotExistException e)
        {
            await RespondFailureAsync($"There is no table associated with the role {role.Mention}.");
            return;
        } catch (KeyAlreadyPresentInTableException e)
        {
            await RespondFailureAsync($"There is already an instance of the {role.Mention} collection for the {keyTypeString} {keyMention}.");
            return;
        }

        await RespondSuccessAsync($"You have successfully instantiated a {role.Mention} collection for {keyTypeString} {keyMention}.");
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
                await RespondFailureAsync($"There is no collection associated with {role.Mention}.");
                return;
            } catch (InvalidNameException e)
            {
                await RespondFailureAsync($"The name `{currencyName}` is invalid.  Please choose a name with only alpha-numeric characters for the currency.");
                return;
            } catch (ColumnAlreadyExistsException e)
            {
                await RespondFailureAsync($"There is already a currency with the name `{currencyName}` in the {role.Mention} collection.  Please choose a different currency name.");
                return;
            }

            await RespondSuccessAsync($"The currency `{currencyName}` has been added to collection {role.Mention}.");
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
                await RespondFailureAsync("You must type `DELETE` after the role and currency name in order to confirm deletion.");
                return;
            }

            try
            {
                _dataAccess.DeleteColumn(Role.Id, CurrencyName);
                await RespondSuccessAsync($"`{CurrencyName}` has been removed from the {Role.Mention} collection.");
            } catch (TableDoesNotExistException e)
            {
                await RespondFailureAsync($"There is no collection assigned to {Role.Mention}.");
                return;
            } catch (ColumnDoesNotExistException e)
            {
                await RespondFailureAsync($"There is no currency named `{CurrencyName}` in the {Role.Mention} collection.  Currencies are CASE-SENSITIVE.");
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
                await RespondSuccessAsync($"The currency `{oldCurrencyName}` has been renamed to `{newCurrencyName}` in the collection {role.Mention}.");
                return;
            }
            catch (TableDoesNotExistException e)
            {
                await RespondFailureAsync($"There is no collection assigned to {role.Mention}.");
                return;
            }
            catch (ColumnDoesNotExistException e)
            {
                await RespondFailureAsync($"There is no currency named `{oldCurrencyName}` in the {role.Mention} collection.  Currencies are CASE-SENSITIVE.");
                return;
            }
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
}