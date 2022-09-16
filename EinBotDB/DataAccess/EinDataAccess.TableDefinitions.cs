namespace EinBotDB.DataAccess;

using EinBotDB.Context;
using EinBotDB.Models;

public partial class EinDataAccess
{
    /// <summary>
    /// Creates a new table.
    /// </summary>
    /// <param name="tableName">The name of the new table.  Must contain only alpha-numeric and '-' characters. </param>
    /// <param name="collectionType">The type of collection this table is.</param>
    /// <param name="roleId">The role id associated with this table.</param>
    /// <returns>The TableDefinitionsModel entity of the created table.</returns>
    /// <exception cref="InvalidNameException"></exception>
    /// <exception cref="TableAlreadyExistsException"></exception>"
    /// /// <exception cref="TableAlreadyExistsWithRoleException"></exception>"
    /// /// <exception cref="TableAlreadyExistsWithNameException"></exception>"
    public TableDefinitionsModel? CreateTable(string tableName, CollectionTypesEnum collectionType, ulong roleId)
    {
        using var context = _factory.CreateDbContext();

        if (string.IsNullOrEmpty(tableName)) throw new InvalidNameException(tableName);

        if (InternalDoesTableExist(context, roleId: roleId)) throw new TableAlreadyExistsWithRoleException(roleId);
        if (InternalDoesTableExist(context, tableName: tableName)) throw new TableAlreadyExistsWithNameException(tableName);


        TableDefinitionsModel tableDefinition = new TableDefinitionsModel()
        {
            Name = tableName,
            CollectionTypeId = (int)collectionType,
            RoleId = roleId
        };

        tableDefinition = context.TableDefinitions.Add(tableDefinition).Entity;
        context.SaveChanges();
        return tableDefinition;
    }

    /*
    /// <summary>
    /// Removes the table with the given table id.
    /// </summary>
    /// <param name="tableId">The id of the table to delete.</param>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given table id.</exception>
    public TableDefinitionsModel DeleteTable(int tableId)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefiniton = context.TableDefinitions.Find(tableId);

        if (tableDefiniton is null) throw new TableDoesNotExistException(tableId);

        tableDefiniton = context.TableDefinitions.Remove(tableDefiniton).Entity;

        context.SaveChanges();

        return tableDefiniton;
    }

    /// <summary>
    /// Removes the table with the given table name.
    /// </summary>
    /// <param name="tableName">The name of the table to delete.</param>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given name.</exception>
    public TableDefinitionsModel DeleteTable(string tableName)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.Name.Equals(tableName));

        if (tableDefinition is null) throw new TableDoesNotExistException(tableName);

        tableDefinition = context.TableDefinitions.Remove(tableDefinition).Entity;

        context.SaveChanges();

        return tableDefinition;
    }

    /// <summary>
    /// Removes the table with the given role id.
    /// </summary>
    /// <param name="roleId">The role id of the table to delete.</param>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given role id.</exception>
    public TableDefinitionsModel DeleteTable(ulong roleId)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.RoleId == roleId);

        if (tableDefinition is null) throw new TableDoesNotExistException(roleId);

        tableDefinition = context.TableDefinitions.Remove(tableDefinition).Entity;

        context.SaveChanges();

        return tableDefinition;
    }*/

    public TableDefinitionsModel DeleteTable(int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel tableDefinition = InternalGetTable(context, tableId: tableId, roleId: roleId, tableName: tableName);

        tableDefinition = context.TableDefinitions.Remove(tableDefinition).Entity;
        context.SaveChanges();
        return tableDefinition;
    }

    /// <summary>
    /// Retrieves a table with the given tableId, roleId, or tableName.
    /// </summary>
    /// <param name="tableId">Null or the id of the table.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">Null or the role id of the table.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="tableName">Null or the name of the table.  If null, then either roleId or tableId cannot be null.</param>
    /// <returns>A TableDefinitionsModel of the table.</returns>
    /// <exception cref="TableDoesNotExistException">If all arguments are null, or if a table cannot be found with the given argument.</exception>
    public TableDefinitionsModel GetTable(int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        using var context = _factory.CreateDbContext();

        return InternalGetTable(context, tableId: tableId, roleId: roleId, tableName: tableName);
    }

    public bool DoesTableExist(int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        using var context = _factory.CreateDbContext();

        return InternalDoesTableExist(context, tableId: tableId, roleId: roleId, tableName: tableName);
    }


    /*
    /// <summary>
    /// Retrieves a table with the given name.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <returns>The TableDefinitionsModel of the table.</returns>
    /// <exception cref="TableDoesNotExistException">If a table with the given table name does not exist.</exception>
    public TableDefinitionsModel GetTable(string tableName)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.Name.Equals(tableName));

        if (tableDefinition is null) throw new TableDoesNotExistException(tableName);

        return tableDefinition;
    }
    


    /// <summary>
    /// Retrieves a table with the given role id.
    /// </summary>
    /// <param name="tableId">Table id of the table.</param>
    /// <returns>The TableDefinitionsModel of the table.</returns>
    /// <exception cref="TableDoesNotExistException">If a table with the given table id does not exist.</exception>
    public TableDefinitionsModel GetTable(int tableId)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.Id == tableId);

        if (tableDefinition is null) throw new TableDoesNotExistException(tableId);

        return tableDefinition;
    }

    /// <summary>
    /// Retrieves a table with the given role id.
    /// </summary>
    /// <param name="roleId">Role id associated with the table.</param>
    /// <returns>The TableDefinitionsModel of the table.</returns>
    /// <exception cref="TableDoesNotExistException">If a table with the given role id does not exist.</exception>
    public TableDefinitionsModel GetTable(ulong roleId)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.RoleId == roleId);

        if (tableDefinition is null) throw new TableDoesNotExistException(roleId);

        return tableDefinition;
    }
    */

    /*
    /// <summary>
    /// Rename a table.
    /// </summary>
    /// <param name="tableId">Table id of the table to rename</param>
    /// <param name="newTableName">Name to set the table to.</param>
    /// <exception cref="InvalidNameException">Thrown if the the table name is invalid.</exception>
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    public string RenameTable(int tableId, string newTableName)
    {
        var name = newTableName.ToAlphaNumericDash();

        if (string.IsNullOrEmpty(name)) throw new InvalidNameException($"Table name: {newTableName}");

        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = GetTable(context, tableId: tableId);

        if (tableDefinition is null) throw new TableDoesNotExistException(tableId);

        var oldName = tableDefinition.Name;

        tableDefinition.Name = name;

        context.SaveChanges();

        return oldName;
    }

    /// <summary>
    /// Rename a table.
    /// </summary>
    /// <param name="tableName">Old name of the table</param>
    /// <param name="newTableName">Name to set the table to.</param>
    /// <exception cref="InvalidNameException">Thrown if the the table name is invalid.</exception>
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    public string RenameTable(string tableName, string newTableName)
    {
        var name = newTableName.ToAlphaNumericDash();

        if (string.IsNullOrEmpty(name)) throw new InvalidNameException($"Table name: {newTableName}");

        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = GetTable(context, tableName: tableName);

        if (tableDefinition is null) throw new TableDoesNotExistException(tableName);

        var oldName = tableDefinition.Name;

        tableDefinition.Name = name;

        context.SaveChanges();

        return oldName;
    }

    /// <summary>
    /// Rename a table.
    /// </summary>
    /// <param name="roleId">Role id of the table to rename</param>
    /// <param name="newTableName">Name to set the table to.</param>
    /// <exception cref="InvalidNameException">Thrown if the the table name is invalid.</exception>
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    public string RenameTable(ulong roleId, string newTableName)
    {
        var name = newTableName.ToAlphaNumericDash();

        if (string.IsNullOrEmpty(name)) throw new InvalidNameException($"Table name: {newTableName}");

        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = GetTable(context, roleId: roleId);

        if (tableDefinition is null) throw new TableDoesNotExistException(roleId);

        var oldName = tableDefinition.Name;

        tableDefinition.Name = name;

        context.SaveChanges();

        return oldName;
    }
    */

    /// <summary>
    /// Sets a table's name.  The name must be unique to this table.
    /// </summary>
    /// <param name="newTableName">The new name of the table.  Must be unique.</param>
    /// <param name="tableId">Null or the id of the table.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">Null or the role id of the table.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="tableName">Null or the name of the table.  If null, then either roleId or tableId cannot be null.</param>
    /// <returns>[string] The old name of the table.</returns>
    /// <exception cref="InvalidNameException">If the given name is invalid.</exception>
    /// <exception cref="TableAlreadyExistsWithNameException">If there is already a table with that name in existance.</exception>
    /// <exception cref="TableDoesNotExistException">If all arguments are null, or if a table cannot be found with the given argument.</exception>
    public string RenameTable(string newTableName,
        int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        if (string.IsNullOrEmpty(newTableName)) throw new InvalidNameException($"Table names cannot be blank or null.");
        
        using var context = _factory.CreateDbContext();
        
        if (InternalDoesTableExist(context, tableName: newTableName)) throw new TableAlreadyExistsWithNameException(newTableName);

        TableDefinitionsModel tableDefinition = InternalGetTable(context, tableId: tableId, roleId: roleId, tableName: tableName);

        string oldName = tableDefinition.Name;
        tableDefinition.Name = newTableName;

        context.SaveChanges();

        return oldName;
    }

    /// <summary>
    /// Sets a new role id for the table.  No other table can have this role id.
    /// </summary>
    /// <param name="newRoleId"></param>
    /// <param name="tableId">Null or the id of the table.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">Null or the role id of the table.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="tableName">Null or the name of the table.  If null, then either roleId or tableId cannot be null.</param>
    /// <exception cref="TableDoesNotExistException">If all arguments are null, or if a table cannot be found with the given argument.</exception>
    /// <exception cref="TableAlreadyExistsWithRoleException">If there's a table which already has this role id.</exception>
    public void SetTableRole(ulong newRoleId,
        int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        using var context = _factory.CreateDbContext();

        if (InternalDoesTableExist(context, roleId: newRoleId)) throw new TableAlreadyExistsWithRoleException(newRoleId);

        TableDefinitionsModel tableDefinition = InternalGetTable(context, tableId: tableId, roleId: roleId, tableName: tableName);

        tableDefinition.RoleId = newRoleId;

        context.SaveChanges();
    }

    /************************************************************
     * 
     * INTERNAL METHODS
     * 
     ************************************************************/

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="tableId"></param>
    /// <param name="roleId"></param>
    /// <param name="tableName"></param>
    /// <returns></returns>
    /// <exception cref="TableDoesNotExistException"></exception>
    internal bool InternalDoesTableExist(EinDataContext context, int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        Func<TableDefinitionsModel, bool>? tableSearchFunc = GetTableSearchFunc(tableId: tableId, roleId: roleId, tableName: tableName);

        if (tableSearchFunc is null) throw new TableDoesNotExistException($"Can't look up NULL table.");

        if (context.TableDefinitions.FirstOrDefault(tableSearchFunc) is null) return false;

        return true;
    }

    /// <summary>
    /// Internal function for retrieving a table in the given context.
    /// </summary>
    /// <param name="context">The db context.</param>
    /// <param name="tableId">Null or the id of the table.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">Null or the role id of the table.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="tableName">Null or the name of the table.  If null, then either roleId or tableId cannot be null.</param>
    /// <returns>A TableDefinitionsModel of the table.</returns>
    /// <exception cref="TableDoesNotExistException">If all arguments are null, or if a table cannot be found with the given argument.</exception>
    internal TableDefinitionsModel InternalGetTable(EinDataContext context, int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        Func<TableDefinitionsModel, bool>? tableSearchFunc = GetTableSearchFunc(tableId: tableId, roleId: roleId, tableName: tableName);

        if (tableSearchFunc is null) throw new TableDoesNotExistException($"Can't look up NULL table.");

        TableDefinitionsModel? tableDefinition;

        tableDefinition = context.TableDefinitions.FirstOrDefault(tableSearchFunc);

        if (tableDefinition is null) throw new TableDoesNotExistException($"No table found with lookup: {(tableId is null ? "" : $"tableId: {tableId}")}{(roleId is null ? "" : $"roleId: {roleId}")}{(string.IsNullOrEmpty(tableName) ? "" : $"tableName: {tableName}")}.");

        return tableDefinition!;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tableId"></param>
    /// <param name="roleId"></param>
    /// <param name="tableName"></param>
    /// <returns>Null if all args are null, otherwise the search func.</returns>
    internal Func<TableDefinitionsModel, bool>? GetTableSearchFunc(int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        if (tableId is null && roleId is null && string.IsNullOrEmpty(tableName)) return null;

        Func<TableDefinitionsModel, bool> tableSearchFunc;

        if (tableId is not null) tableSearchFunc = (table => table.Id == tableId);
        else if (roleId is not null) tableSearchFunc = (table => table.RoleId == roleId);
        else tableSearchFunc = (table => table.Name.Equals(tableName));

        return tableSearchFunc;
    }
}
