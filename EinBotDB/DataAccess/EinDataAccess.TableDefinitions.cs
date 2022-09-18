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
    public TableDefinitionsModel CreateTable(string tableName, CollectionTypesEnum collectionType, ulong roleId)
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

    /// <summary>
    /// Deletes a table.
    /// </summary>
    /// <param name="tableId">NULL or the id of the table to delete.  If null, then either role id or tablename cannot be.</param>
    /// <param name="roleId">NULL or the role id of the table to delete.  If null, then either table id or tablename cannot be.</param>
    /// <param name="tableName">NULL or the name of the table to delete.  If null, then either role id or table id cannot be.</param>
    /// <returns>A <class cref="TableDefinitionsModel">TableDefinitionsModel</class> of the deleted table.</returns>
    public TableDefinitionsModel DeleteTable(int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel tableDefinition = InternalGetTable(context, tableId: tableId, roleId: roleId, tableName: tableName);

        tableDefinition = context.TableDefinitions.Remove(tableDefinition).Entity;
        context.SaveChanges();
        return tableDefinition;
    }

    /// <summary>
    /// Checks if a table with the given identifiers exists.
    /// </summary>
    /// <param name="tableId">NULL or the id of the table.  If null, then either role id or table name must not be null.</param>
    /// <param name="roleId">NULL or the role id of the table.  If null, then either table id or table name must not be null.</param>
    /// <param name="tableName">NULL or the name of thet able.  If nole then either table id or role id must not be null.</param>
    /// <returns>True if table exists, otherwise false.</returns>
    public bool DoesTableExist(int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        using var context = _factory.CreateDbContext();

        return InternalDoesTableExist(context, tableId: tableId, roleId: roleId, tableName: tableName);
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
