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
    public void DeleteTable(int tableId)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefiniton = context.TableDefinitions.Find(tableId);

        if (tableDefiniton is null) throw new TableDoesNotExistException(tableId);

        context.TableDefinitions.Remove(tableDefiniton);

        context.SaveChanges();
    }

    /// <summary>
    /// Removes the table with the given table name.
    /// </summary>
    /// <param name="tableName">The name of the table to delete.</param>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given name.</exception>
    public void DeleteTable(string tableName)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.Name.Equals(tableName));

        if (tableDefinition is null) throw new TableDoesNotExistException(tableName);

        context.TableDefinitions.Remove(tableDefinition);

        context.SaveChanges();
    }

    /// <summary>
    /// Removes the table with the given role id.
    /// </summary>
    /// <param name="roleId">The role id of the table to delete.</param>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given role id.</exception>
    public void DeleteTable(ulong roleId)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.RoleId == roleId);

        if (tableDefinition is null) throw new TableDoesNotExistException(roleId);

        context.TableDefinitions.Remove(tableDefinition);

        context.SaveChanges();
    }

    /// <summary>
    /// Rename a table.
    /// </summary>
    /// <param name="tableId">Table id of the table to rename</param>
    /// <param name="newTableName">Name to set the table to.</param>
    /// <exception cref="InvalidNameException">Thrown if the the table name is invalid.</exception>
    public void RenameTable(int tableId, string newTableName)
    {
        var name = newTableName.ToAlphaNumericDash();

        if (string.IsNullOrEmpty(name)) throw new InvalidNameException($"Table name: {newTableName}");

        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = GetTable(context, tableId: tableId);

        tableDefinition.Name = name;

        context.SaveChanges();
    }

    /// <summary>
    /// Rename a table.
    /// </summary>
    /// <param name="tableName">Old name of the table</param>
    /// <param name="newTableName">Name to set the table to.</param>
    /// <exception cref="InvalidNameException">Thrown if the the table name is invalid.</exception>
    public void RenameTable(string tableName, string newTableName)
    {
        var name = newTableName.ToAlphaNumericDash();

        if (string.IsNullOrEmpty(name)) throw new InvalidNameException($"Table name: {newTableName}");

        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = GetTable(context, tableName: tableName);

        tableDefinition.Name = name;

        context.SaveChanges();
    }

    /// <summary>
    /// Rename a table.
    /// </summary>
    /// <param name="roleId">Role id of the table to rename</param>
    /// <param name="newTableName">Name to set the table to.</param>
    /// <exception cref="InvalidNameException">Thrown if the the table name is invalid.</exception>
    public void RenameTable(ulong roleId, string newTableName)
    {
        var name = newTableName.ToAlphaNumericDash();

        if (string.IsNullOrEmpty(name)) throw new InvalidNameException($"Table name: {newTableName}");

        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = GetTable(context, roleId: roleId);

        tableDefinition.Name = name;

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
