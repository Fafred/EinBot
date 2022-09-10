namespace EinBotDB.DataAccess;

using System.Linq;
using EinBotDB.Context;
using EinBotDB.Exceptions;
using EinBotDB.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public partial class EinDataAccess
{
    /// <summary>
    /// Create a new column for the given table.
    /// </summary>
    /// <param name="tableId">The id of the table this column should be placed in.</param>
    /// <param name="columnName">The name of the column.  Everything other than alpha-numerics and '-' chars are stripped out.</param>
    /// <param name="dataType">The type of data stored in this column</param>
    /// <returns>A ColumnDefinitionModel entity returned by the db context.</returns>
    /// <exception cref="TableDoesNotExistException">If the table the column was to be added to does not exist.</exception>
    /// <exception cref="InvalidNameException">If the name of the column is invalid.</exception>
    /// <exception cref="ColumnAlreadyExistsException">If there is already a column with that name in the given table.</exception>
    public ColumnDefinitionsModel? CreateColumn(int tableId, string columnName, DataTypesEnum dataType)
    {
        using var context = _factory.CreateDbContext();
        return CreateColumnHelper(tableId, columnName, dataType, context);
    }

    /// <summary>
    /// Create a new column for the given table.
    /// </summary>
    /// <param name="tableName">The name of the table this column should be placed in.</param>
    /// <param name="columnName">The name of the column.  Everything other than alpha-numerics and '-' chars are stripped out.</param>
    /// <param name="dataType">The type of data stored in this column</param>
    /// <returns>A ColumnDefinitionModel entity returned by the db context.</returns>
    /// <exception cref="TableDoesNotExistException">If the table the column was to be added to does not exist.</exception>
    /// <exception cref="InvalidNameException">If the name of the column is invalid.</exception>
    /// <exception cref="ColumnAlreadyExistsException">If there is already a column with that name in the given table.</exception>
    public ColumnDefinitionsModel? CreateColumn(string tableName, string columnName, DataTypesEnum dataType)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinitions = context.TableDefinitions.FirstOrDefault(x => x.Name == tableName);

        if (tableDefinitions is null) throw new TableDoesNotExistException(tableName);

        return CreateColumnHelper(tableDefinitions.Id, columnName, dataType, context);
    }

    /// <summary>
    /// Create a new column for the given table.
    /// </summary>
    /// <param name="roleId">The role id of the table this column should be placed in.</param>
    /// <param name="columnName">The name of the column.  Everything other than alpha-numerics and '-' chars are stripped out.</param>
    /// <param name="dataType">The type of data stored in this column</param>
    /// <returns>A ColumnDefinitionModel entity returned by the db context.</returns>
    /// <exception cref="TableDoesNotExistException">If the table the column was to be added to does not exist.</exception>
    /// <exception cref="InvalidNameException">If the name of the column is invalid.</exception>
    /// <exception cref="ColumnAlreadyExistsException">If there is already a column with that name in the given table.</exception>
    public ColumnDefinitionsModel? CreateColumn(ulong roleId, string columnName, DataTypesEnum dataType)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinitions = context.TableDefinitions.FirstOrDefault(x => x.RoleId == roleId);

        if (tableDefinitions is null) throw new TableDoesNotExistException(roleId);

        return CreateColumnHelper(tableDefinitions.Id, columnName, dataType, context);
    }

    /// <summary>
    /// Attempts to delete a column with the given id.
    /// </summary>
    /// <param name="columnId">The id of the column to delete</param>
    /// <returns>A ColumnDefinitionsModel entity of the deleted column.</returns>
    /// <exception cref="ColumnDoesNotExistException">If there is no column with the given id.</exception>
    public ColumnDefinitionsModel DeleteColumn(int columnId)
    {
        var context = _factory.CreateDbContext();

        ColumnDefinitionsModel? columnDefinition = context.ColumnDefinitions.FirstOrDefault(x => x.Id == columnId);

        if (columnDefinition is null) throw new ColumnDoesNotExistException("Unknown", columnId);

        return context.ColumnDefinitions.Remove(columnDefinition).Entity;
    }

    /// <summary>
    /// Attempts to delete a column with the given id.
    /// </summary>
    /// <param name="tableId">The id of the table the column is in.</param>
    /// <param name="columnName">The name of the column to delete.</param>
    /// <returns>A ColumnDefinitionsModel entity of the deleted column.</returns>
    /// <exception cref="ColumnDoesNotExistException">If there is no column with the given id.</exception>
    public ColumnDefinitionsModel DeleteColumn(int tableId, string columnName)
    {
        var context = _factory.CreateDbContext();

        ColumnDefinitionsModel? columnDefinition = context.ColumnDefinitions.FirstOrDefault(x => x.TableDefinitionsId == tableId && x.Name.Equals(columnName));

        if (columnDefinition is null) throw new ColumnDoesNotExistException(tableId.ToString(), columnName);

        return context.Remove(columnDefinition).Entity;
    }

    /// <summary>
    /// Attempts to delete a column with the given name from the table with the given name.
    /// </summary>
    /// <param name="tableRoleId">The role id of the table the column is on</param>
    /// <param name="columnName">The name of the column to delete.</param>
    /// <returns>A ColumnDefinitionsModel entity of the deleted column.</returns>
    /// <exception cref="ColumnDoesNotExistException">If there is no column with the given role id.</exception>
    /// <exception cref="TableDoesNotExistException">If there is no table with the given role id.</exception>
    public ColumnDefinitionsModel DeleteColumn(ulong tableRoleId, string columnName)
    {
        var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinitions = context.TableDefinitions.FirstOrDefault(table => table.RoleId == tableRoleId);

        if (tableDefinitions is null) throw new TableDoesNotExistException(tableRoleId);

        var tableId = tableDefinitions.Id;

        ColumnDefinitionsModel? columnDefinition = context.ColumnDefinitions.FirstOrDefault(x => x.TableDefinitionsId == tableId && x.Name.Equals(columnName));

        if (columnDefinition is null) throw new ColumnDoesNotExistException(tableId.ToString(), columnName);

        return context.Remove(columnDefinition).Entity;
    }

    /// <summary>
    /// Attempts to delete a column with the given name from the table with the given name.
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="columnName">The name of the column to delete.</param>
    /// <returns>A ColumnDefinitionsModel entity of the deleted column.</returns>
    /// <exception cref="ColumnDoesNotExistException">If there is no column with the given column id.</exception>
    /// <exception cref="TableDoesNotExistException">If there is no table with the given table name.</exception>
    public ColumnDefinitionsModel DeleteColumn(string tableName, string columnName)
    {
        var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinitions = context.TableDefinitions.FirstOrDefault(table => table.Name.Equals(tableName));

        if (tableDefinitions is null) throw new TableDoesNotExistException(tableName);

        var tableId = tableDefinitions.Id;

        ColumnDefinitionsModel? columnDefinition = context.ColumnDefinitions.FirstOrDefault(column => column.TableDefinitionsId == tableId && column.Name.Equals(columnName));

        if (columnDefinition is null) throw new ColumnDoesNotExistException(tableId.ToString(), columnName);

        return context.Remove(columnDefinition).Entity;
    }

    /// <summary>
    /// Renames a column.
    /// </summary>
    /// <param name="columnId">The id of the column to rename.</param>
    /// <param name="newColumnName">The new name to set the column to.</param>
    /// <returns>A ColumnDefinitionsModel of the column which was renamed.</returns>
    /// <exception cref="ColumnDoesNotExistException">If a column with the given id doesn't exist.</exception>
    /// <exception cref="InvalidNameException">If the newColumnName is invalid (only alpha-numeric and '-' allowed.</exception>
    public ColumnDefinitionsModel RenameColumn(int columnId, string newColumnName)
    {
        var context = _factory.CreateDbContext();

        ColumnDefinitionsModel? columnDefinition = context.ColumnDefinitions.FirstOrDefault(column => column.Id == columnId);

        if (columnDefinition is null) throw new ColumnDoesNotExistException("Unknown", columnId);

        var columnName = newColumnName.ToAlphaNumericDash().Trim();

        if (string.IsNullOrEmpty(columnName)) throw new InvalidNameException(newColumnName);

        columnDefinition.Name = columnName;

        context.SaveChanges();

        return columnDefinition;
    }

    /// <summary>
    /// Renames a column.
    /// </summary>
    /// <param name="tableId">The id associated with the table.</param>
    /// <param name="oldColumnName">The current name of the column to rename.</param>
    /// <param name="newColumnName">The new name to set the column to.  Only '-' and alpha-numeric allowed.</param>
    /// <returns>A ColumnDefinitionsModel of the column which was renamed.</returns>
    /// <exception cref="TableDoesNotExistException">If no table exists with the given id.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the column with the given oldColumnName doesn't exist.</exception>
    /// <exception cref="InvalidNameException">If the new column name is invalid.</exception>"
    public ColumnDefinitionsModel RenameColumn(int tableId, string oldColumnName, string newColumnName)
    {
        var context = _factory.CreateDbContext();

        return RenameColumnHelper(tableId, oldColumnName, newColumnName, context);
    }

    /// <summary>
    /// Renames a column.
    /// </summary>
    /// <param name="tableName">The name associated with the table.</param>
    /// <param name="oldColumnName">The current name of the column to rename.</param>
    /// <param name="newColumnName">The new name to set the column to.  Only '-' and alpha-numeric allowed.</param>
    /// <returns>A ColumnDefinitionsModel of the column which was renamed.</returns>
    /// <exception cref="TableDoesNotExistException">If no table exists with the given name.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the column with the given oldColumnName doesn't exist.</exception>
    /// <exception cref="InvalidNameException">If the new column name is invalid.</exception>"
    public ColumnDefinitionsModel RenameColumn(string tableName, string oldColumnName, string newColumnName)
    {
        var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.Name.Equals(tableName);

        if (tableDefinition is null) throw new TableDoesNotExistException(tableName);

        return RenameColumnHelper(tableDefinition.Id, oldColumnName, newColumnName, context);
    }

    /// <summary>
    /// Renames a column.
    /// </summary>
    /// <param name="tableRoleId">The role id associated with the table.</param>
    /// <param name="oldColumnName">The current name of the column to rename.</param>
    /// <param name="newColumnName">The new name to set the column to.  Only '-' and alpha-numeric allowed.</param>
    /// <returns>A ColumnDefinitionsModel of the column which was renamed.</returns>
    /// <exception cref="TableDoesNotExistException">If not able exists with the given id.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the column with the given oldColumnName doesn't exist.</exception>
    /// <exception cref="InvalidNameException">If the new column name is invalid.</exception>"
    public ColumnDefinitionsModel RenameColumn(ulong tableRoleId, string oldColumnName, string newColumnName)
    {
        var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.RoleId == tableRoleId);

        if (tableDefinition is null) throw new TableDoesNotExistException(tableRoleId);

        return RenameColumnHelper(tableDefinition.Id, oldColumnName, newColumnName, context);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                         PRIVATE METHODS.                                         ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Helper method for the RenameColumn methods.
    /// </summary>
    /// <param name="tableId">Table id of the table the column is on.</param>
    /// <param name="oldColumnName">The old column name.</param>
    /// <param name="newColumnName">The new column name.  Only '-' and alpha-numeric allowed.</param>
    /// <param name="context">The dbcontext</param>
    /// <returns>A ColumnDefinitionsModel of the column which was renamed.</returns>
    /// <exception cref="TableDoesNotExistException">If the table with the given id doesn't exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the column with the given oldColumnName doesn't exist.</exception>
    /// <exception cref="InvalidNameException">If the new column name is invalid.</exception>"
    private ColumnDefinitionsModel RenameColumnHelper(int tableId, string oldColumnName, string newColumnName, EinDataContext context)
    {
        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.Id == tableId);

        if (tableDefinition is null) throw new TableDoesNotExistException(tableId);

        ColumnDefinitionsModel? columnDefinition = context.ColumnDefinitions.FirstOrDefault(column => column.Name.Equals(oldColumnName) && column.TableDefinitionsId == tableId);

        if (columnDefinition is null) throw new ColumnDoesNotExistException(tableDefinition.Name, oldColumnName);

        var columnName = newColumnName.ToAlphaNumericDash().Trim();

        if (string.IsNullOrEmpty(columnName)) throw new InvalidNameException(newColumnName);

        columnDefinition.Name = columnName;

        context.SaveChanges();

        return columnDefinition;
    }

    /// <summary>
    /// Helper method for creating columns.
    /// </summary>
    /// <param name="tableId">The id of the table this column should be placed in.</param>
    /// <param name="columnName">The name of the column.  Everything other than alpha-numerics and '-' chars are stripped out.</param>
    /// <param name="dataType">The type of data stored in this column</param>
    /// <param name="context">The database context</param>
    /// <returns>A ColumnDefinitionModel entity returned by the db context.</returns>
    /// <exception cref="TableDoesNotExistException">If the table the column was to be added to does not exist.</exception>
    /// <exception cref="InvalidNameException">If the name of the column is invalid.</exception>
    /// <exception cref="ColumnAlreadyExistsException">If there is already a column with that name in the given table.</exception>
    private ColumnDefinitionsModel? CreateColumnHelper(int tableId, string columnName, DataTypesEnum dataType, EinDataContext context)
    {
        if (context.TableDefinitions.FirstOrDefault(x => x.Id == tableId) == null) throw new TableDoesNotExistException(tableId);

        // Strip unacceptable characters.
        var name = columnName.ToAlphaNumericDash();

        if (string.IsNullOrEmpty(columnName)) throw new InvalidNameException($"Column name {columnName}");

        var columnNames = (from column in context.ColumnDefinitions
                           where column.TableDefinitionsId == tableId
                           select column.Name).ToList();

        if (columnNames.Contains(name)) throw new ColumnAlreadyExistsException(tableId, name);


        ColumnDefinitionsModel columnDefinitionsModel = new ColumnDefinitionsModel()
        {
            Name = name,
            TableDefinitionsId = tableId,
            DataTypesId = (int)dataType,
        };

        columnDefinitionsModel = context.ColumnDefinitions.Add(columnDefinitionsModel).Entity;
        context.SaveChanges();

        return columnDefinitionsModel;
    }


}
