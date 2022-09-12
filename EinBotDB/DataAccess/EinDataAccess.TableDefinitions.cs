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
    public TableDefinitionsModel? CreateTable(string tableName, CollectionTypesEnum collectionType, ulong? roleId = null)
    {
        using var context = _factory.CreateDbContext();

        var name = tableName.ToAlphaNumericDash().Trim();

        if (string.IsNullOrEmpty(name)) throw new InvalidNameException($"Table name: {tableName}");

        TableDefinitionsModel tableDefinition = new TableDefinitionsModel()
        {
            Name = name,
            CollectionTypeId = (int)collectionType,
            RoleId = roleId
        };

        tableDefinition = context.TableDefinitions.Add(tableDefinition).Entity;
        context.SaveChanges();
        return tableDefinition;
    }

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
    }

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

    public void SetTableRole(ulong oldRoleId, ulong newRoleId)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = GetTable(context, roleId: oldRoleId);

        if (tableDefinition is null) throw new TableDoesNotExistException(oldRoleId);

        tableDefinition.RoleId = newRoleId;

        context.SaveChanges();
    }

    /// <summary>
    /// Helper method to get tables.
    /// </summary>
    /// <param name="context">db context</param>
    /// <param name="tableId"></param>
    /// <param name="tableName"></param>
    /// <param name="roleId"></param>
    /// <returns>A TableDefinitionsModel</returns>
    /// <exception cref="TableDoesNotExistException"></exception>
    private TableDefinitionsModel GetTable(EinDataContext context, int? tableId = null, string? tableName = null, ulong? roleId = null)
    {
        TableDefinitionsModel? tableDefinitions;

        if (tableId is not null)
        {
            tableDefinitions = context.TableDefinitions.FirstOrDefault(table => table.Id == tableId);

            if (tableDefinitions is null) throw new TableDoesNotExistException((int)tableId);
        } else if (tableName is not null)
        {
            tableDefinitions = context.TableDefinitions.FirstOrDefault(table => table.Name.Equals(tableName));
            if (tableDefinitions is null) throw new TableDoesNotExistException(tableName);
        } else if (roleId is not null)
        {
            tableDefinitions = context.TableDefinitions.FirstOrDefault(table => table.RoleId == roleId);
            if (tableDefinitions is null) throw new TableDoesNotExistException((ulong)roleId);
        } else
        {
            throw new TableDoesNotExistException("NULL");
        }

        return tableDefinitions;
    }

}
