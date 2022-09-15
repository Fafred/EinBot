namespace EinBotDB.DataAccess;

using System.Linq;
using System.Security.Cryptography;
using EinBotDB.Context;
using EinBotDB.Models;

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
        return CreateColumnHelper(columnName, dataType, tableId: tableId);
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
        return CreateColumnHelper(columnName, dataType, tableName: tableName);
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
        return CreateColumnHelper(columnName, dataType, roleId: roleId);
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

        columnDefinition = context.Remove(columnDefinition).Entity;

        context.SaveChanges();

        return columnDefinition;
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

        columnDefinition = context.Remove(columnDefinition).Entity;

        context.SaveChanges();

        return columnDefinition;
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

        columnDefinition = context.Remove(columnDefinition).Entity;

        context.SaveChanges();

        return columnDefinition;
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

        columnDefinition = context.Remove(columnDefinition).Entity;

        context.SaveChanges();

        return columnDefinition;
    }

    /// <summary>
    /// Retrieves the columns of a table.
    /// </summary>
    /// <param name="tableId">id of table the columns are in.</param>
    /// <returns>List of ColumnDefinitionsModel.</returns>
    /// <exception cref="TableDoesNotExistException">If no table exists with the given id.</exception>
    public List<ColumnDefinitionsModel> GetColumns(int tableId)
    {
        var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.Id == tableId);

        if (tableDefinition is null) throw new TableDoesNotExistException(tableId);

        var columnsList = context.ColumnDefinitions.Where(column => column.TableDefinitionsId == tableId).ToList();

        return columnsList;
    }

    /// <summary>
    /// Retrieves the columns of a table.
    /// </summary>
    /// <param name="roleId">role id of table the columns are in.</param>
    /// <returns>List of ColumnDefinitionsModel.</returns>
    /// <exception cref="TableDoesNotExistException">If no table exists with the given role id.</exception>
    public List<ColumnDefinitionsModel> GetColumns(ulong roleId)
    {
        var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.RoleId == roleId);

        if (tableDefinition is null) throw new TableDoesNotExistException(roleId);

        var columnsList = context.ColumnDefinitions.Where(column => column.TableDefinitionsId == tableDefinition.Id).ToList();

        return columnsList;
    }

    /// <summary>
    /// Retrieves the columns of a table.
    /// </summary>
    /// <param name="tableName">name of table the columns are in.</param>
    /// <returns>List of ColumnDefinitionsModel.</returns>
    /// <exception cref="TableDoesNotExistException">If no table exists with the given name.</exception>
    public List<ColumnDefinitionsModel> GetColumns(string tableName)
    {
        var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.Name.Equals(tableName));

        if (tableDefinition is null) throw new TableDoesNotExistException(tableName);

        var columnsList = context.ColumnDefinitions.Where(column => column.TableDefinitionsId == tableDefinition.Id).ToList();

        return columnsList;
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

        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.Name.Equals(tableName));

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


    /// <summary>
    /// A helper method for creating columns.  If there are already rows in the cells table for the table this column is going in, it will create
    /// new cells for the new column for each existing row in that table.
    /// </summary>
    /// <param name="columnName">The name of the column to create.</param>
    /// <param name="dataType">The data type of the column.</param>
    /// <param name="tableId">Null or the id of the table to create the column in.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">Null or the role id of the table to create the column in.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">Null or the table name of the table to create the column in.  If null then either tableId or roleId cannot be null.</param>
    /// <returns>A ColumnDefinitionsModel of the new column.</returns>
    /// <exception cref="TableDoesNotExistException">If the table the column was to be added to does not exist.</exception>
    /// <exception cref="InvalidNameException">If the name of the column is invalid.</exception>
    /// <exception cref="ColumnAlreadyExistsException">If there is already a column with that name in the given table.</exception>
    private ColumnDefinitionsModel? CreateColumnHelper(string columnName, DataTypesEnum dataType, int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        if (tableId is null && roleId is null && tableName is null) return null;

        using var context = _factory.CreateDbContext();

        TableDefinitionsModel? tableDefinition;
        Func<TableDefinitionsModel, bool> tableSearchFunc;

        if (tableId is not null) tableSearchFunc = (table => table.Id == tableId);
        else if (roleId is not null) tableSearchFunc = (table => table.RoleId == roleId);
        else tableSearchFunc = (table => table.Name.Equals(tableName));

        tableDefinition = context.TableDefinitions.FirstOrDefault(tableSearchFunc);

        if (tableDefinition is null) throw new TableDoesNotExistException(tableId: tableId, roleId: roleId, tableName: tableName);

        tableId = tableDefinition.Id;
        roleId = tableDefinition.RoleId;
        tableName = tableDefinition.Name;

        // Strip unacceptable characters.
        var name = columnName.ToAlphaNumericDash();

        if (string.IsNullOrEmpty(name)) throw new InvalidNameException($"Column name {columnName}");

        var columnNames = (from column in context.ColumnDefinitions
                           where column.TableDefinitionsId == tableId
                           select column.Name).ToList();

        if (columnNames.Contains(name)) throw new ColumnAlreadyExistsException((int)tableId, name);

        // Now create the column.
        ColumnDefinitionsModel columnDefinitionsModel = new ColumnDefinitionsModel()
        {
            Name = name,
            TableDefinitionsId = (int)tableId,
            DataTypesId = (int)dataType,
        };

        columnDefinitionsModel = context.ColumnDefinitions.Add(columnDefinitionsModel).Entity;
        context.SaveChanges();

        // Now we go through and check if there are already rows in the Cells table for this table.
        var cellRows = (from cell in context.Cells
                        where cell.TableDefinitionsId == tableId
                        select new { cell.RowNum, cell.RowKey }).Distinct();

        foreach(var cellData in cellRows)
        {
            CellsModel cellsModel = new CellsModel()
            {
                TableDefinitionsId = tableDefinition.Id,
                ColumnDefinitionsId = columnDefinitionsModel.Id,
                RowKey = cellData.RowKey,
                RowNum = cellData.RowNum,
                Data = "",
            };

            context.Cells.Add(cellsModel);
        }

        context.SaveChanges();

        return columnDefinitionsModel;
    }

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

}
