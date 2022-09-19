namespace EinBotDB.DataAccess;

using EinBotDB;
using EinBotDB.Context;
using EinBotDB.Models;
using System.Linq;

public partial class EinDataAccess
{
    /// <summary>
    /// Create a new column for the given table.
    /// </summary>
    /// <param name="tableId">Null or the id of the table this column should be placed in.  If null, then either roleId or tableName must not be null.</param>
    /// <param name="roleId">Null or the role id of the table this column should be placed in.  If null, then either tableId or tableName must not be null.</param>
    /// <param name="tableName">Null or the name of the table this column should be placed in.  If null, then either tableId or roleId must not be null.</param>
    /// <param name="columnName">The name of the column.  Everything other than alpha-numerics and '-' chars are stripped out.</param>
    /// <param name="dataType">The type of data stored in this column</param>
    /// <returns>A ColumnDefinitionModel entity returned by the db context.</returns>
    /// <exception cref="TableDoesNotExistException">If the table the column was to be added to does not exist.</exception>
    /// <exception cref="InvalidNameException">If the name of the column is invalid.</exception>
    /// <exception cref="ColumnAlreadyExistsException">If there is already a column with that name in the given table.</exception>
    public ColumnDefinitionsModel CreateColumn(string columnName, DataTypesEnum dataType,
        int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        using var context = _factory.CreateDbContext();

        return InternalCreateColumn(context, columnName, dataType, tableID: tableId, roleId: roleId, tableName: tableName);
    }


    /// <summary>
    /// Deletes a column.  May either supply a column id, or else a column name + a table identifier (id, role id, table name).
    /// </summary>
    /// <param name="columnName">The name of the column. If null, then columnId must not be null.</param>
    /// <param name="columnId">Null or the id of the column.  If null, then columnName cannot be NULL and one of the table identifiers must also not be null.</param>
    /// <param name="tableId">Null or the id of the table this column should be placed in.  If null, then either roleId or tableName must not be null if columnId is null.</param>
    /// <param name="roleId">Null or the role id of the table this column should be placed in.  If null, then either tableId or tableName must not be null if columnId is null.</param>
    /// <param name="tableName">Null or the name of the table this column should be placed in.  If null, then either tableId or roleId must not be null if columnId is null.</param>
    /// <returns>A ColumnDefinitionModel entity returned by the db context after it removes the column.</returns>
    /// <exception cref="TableDoesNotExistException">If the table the column was to be removed from does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If there is no column that matches the given id or name + tableid.</exception>
    public ColumnDefinitionsModel DeleteColumn(
        string? columnName = null, int? columnId = null,
        int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        if (string.IsNullOrEmpty(columnName) && columnId is null) throw new ColumnDoesNotExistException("[NULL] column.");
        if (tableId is null && roleId is null && string.IsNullOrEmpty(tableName) && columnId is null) throw new TableDoesNotExistException("[NULL] table");

        using var context = _factory.CreateDbContext();

        ColumnDefinitionsModel columnDefinition = InternalGetColumn(context, columnName, columnId, tableId, roleId, tableName);

        columnDefinition = context.ColumnDefinitions.Remove(columnDefinition).Entity;
        context.SaveChanges();
        return columnDefinition;
    }

    /// <summary>
    /// Retrieves a column of the given table..
    /// </summary>
    /// <param name="columnName">Null or the name of the column to retrieve. If null then columnId cannot be null.</param>
    /// <param name="columnId">Null or the id of the column to retrieve.  If null then columnName cannot be null</param>
    /// <param name="tableId">Null or the id of the table this column is in.  If null, then either roleId or tableName must not be null if columnId is null.</param>
    /// <param name="roleId">Null or the role id of the table this column is in.  If null, then either tableId or tableName must not be null if columnId is null.</param>
    /// <param name="tableName">Null or the name of the table this column is in.  If null, then either tableId or roleId must not be null if columnId is null.</param>
    /// <returns>The ColumnDefinitionModel of the column.</returns>
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the column does not exist.</exception>"
    public ColumnDefinitionsModel GetColumn(
        string? columnName = null, int? columnId = null,
        int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        using var context = _factory.CreateDbContext();

        return InternalGetColumn(context, columnName, columnId, tableId, roleId, tableName);
    }

    /// <summary>
    /// Retrieves all the columns of the given table..
    /// </summary>
    /// <param name="tableId">Null or the id of the table to get the columns from.  If null, then either roleId or tableName must not be null if columnId is null.</param>
    /// <param name="roleId">Null or the role id of the table to get the columns from.  If null, then either tableId or tableName must not be null if columnId is null.</param>
    /// <param name="tableName">Null or the name of the table to get the columns from.  If null, then either tableId or roleId must not be null if columnId is null.</param>
    /// <returns>A list of all the columns in the table.</returns>
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    public List<ColumnDefinitionsModel> GetColumns(
        int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        using var context = _factory.CreateDbContext();

        TableDefinitionsModel tableDefinition = InternalGetTable(context, tableId, roleId, tableName);
        int tableID = tableDefinition.Id;

        List<ColumnDefinitionsModel> columns = context.ColumnDefinitions.Where(column => column.TableDefinitionsId == tableID).ToList();

        return columns ?? new List<ColumnDefinitionsModel>();
    }

    /// <summary>
    /// Renames a column.
    /// </summary>
    /// <param name="newColumnName">The new column name.  Cannot be null or empty, and will be stripped of non-alpha-numerics and non-dashes.</param>
    /// <param name="oldColumnName">NULL or the name of the column to rename.  If null, columnID may not be null.</param>
    /// <param name="columnId">NULL or the id of the column.  Cannot be null if columnName is null.  If given, then the table identifiers may all be null.</param>
    /// <param name="tableId">NULL or the id of the table the column is on.  Only necessary if columnID is null.</param>
    /// <param name="roleId">NULL or the role id of the table the column is on.  Only necessary if columnID is null.</param>
    /// <param name="tableName">NULL or the name of the table the column is on.  Only necessary if columnID is null.</param>
    /// <returns></returns>
    /// <exception cref="InvalidNameException">If the name, after stripping all unallowed chars, is null or empty.</exception>
    /// <exception cref="TableDoesNotExistException">If the table the column is on does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If there is no column that matches the given id or name + table(id/role id/name).</exception>
    public string RenameColumn(
        string newColumnName,
        string? oldColumnName = null, int? columnId = null,
        int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        string scrubbedNewName = newColumnName.ToAlphaNumericDash();
        if (string.IsNullOrEmpty(scrubbedNewName)) throw new InvalidNameException("Column names cannot be null.");

        using var context = _factory.CreateDbContext();

        ColumnDefinitionsModel columnDefinition = InternalGetColumn(context, oldColumnName, columnId, tableId, roleId, tableName);

        string oldName = columnDefinition.Name;

        columnDefinition.Name = scrubbedNewName;

        context.SaveChanges();

        return oldName;
    }

    /******************************************************************************
     * 
     * Internals.
     * 
     *****************************************************************************/

    /// <summary>
    /// Internal create a new column for the given table.
    /// </summary>
    /// <param name="tableID">Null or the id of the table this column should be placed in.  If null, then either roleId or tableName must not be null.</param>
    /// <param name="roleId">Null or the role id of the table this column should be placed in.  If null, then either tableId or tableName must not be null.</param>
    /// <param name="tableName">Null or the name of the table this column should be placed in.  If null, then either tableId or roleId must not be null.</param>
    /// <param name="columnName">The name of the column.  Everything other than alpha-numerics and '-' chars are stripped out.</param>
    /// <param name="dataType">The type of data stored in this column</param>
    /// <returns>A ColumnDefinitionModel entity returned by the db context.</returns>
    /// <exception cref="TableDoesNotExistException">If the table the column was to be added to does not exist.</exception>
    /// <exception cref="InvalidNameException">If the name of the column is invalid.</exception>
    /// <exception cref="ColumnAlreadyExistsException">If there is already a column with that name in the given table.</exception>
    internal ColumnDefinitionsModel InternalCreateColumn(EinDataContext context, string columnName, DataTypesEnum dataType, int? tableID = null, ulong? roleId = null, string? tableName = null)
    {
        string scrubbedName = columnName.ToAlphaNumericDash();
        if (string.IsNullOrEmpty(scrubbedName)) throw new InvalidNameException(columnName);

        TableDefinitionsModel tableDefinition = InternalGetTable(context, tableId: tableID, roleId: roleId, tableName: tableName);

        int tableId = tableDefinition.Id;

        if (InternalDoesColumnExist(context, columnName: scrubbedName, tableID: tableId))
            throw new ColumnAlreadyExistsException(tableId, scrubbedName);

        // Now create the column.
        ColumnDefinitionsModel columnDefinitionsModel = new ColumnDefinitionsModel()
        {
            Name = scrubbedName,
            TableDefinitionsId = tableId,
            DataTypesId = (int)dataType,
        };

        columnDefinitionsModel = context.ColumnDefinitions.Add(columnDefinitionsModel).Entity;
        context.SaveChanges();

        // Now we go through and check if there are already rows in the Cells table for this table.
        var cellRows = (from cell in context.Cells
                        where cell.TableDefinitionsId == tableId
                        select new { cell.RowNum, cell.RowKey }).Distinct();

        foreach (var cellData in cellRows)
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

    internal bool InternalDoesColumnExist(
        EinDataContext context,
        string? columnName = null, int? columnId = null,
        int? tableID = null, ulong? roleId = null, string? tableName = null)
    {
        Func<ColumnDefinitionsModel, bool> columnSearchFunc;

        try
        {

            columnSearchFunc = InternalGetColumnSearchFunc(
                                context,
                                columnName: columnName, columnId: columnId,
                                tableId: tableID, roleId: roleId, tableName: tableName);
        }
        catch (TableDoesNotExistException)
        {
            return false;
        }

        ColumnDefinitionsModel? columnDefinitionsModel = context.ColumnDefinitions.FirstOrDefault(columnSearchFunc);

        if (columnDefinitionsModel is null) return false;

        return true;
    }

    internal ColumnDefinitionsModel InternalGetColumn(
        EinDataContext context,
        string? columnName = null, int? columnId = null,
        int? tableID = null, ulong? roleId = null, string? tableName = null)
    {
        var searchFunc = InternalGetColumnSearchFunc(context, columnName, columnId, tableID, roleId, tableName);

        ColumnDefinitionsModel? columnDefinition = context.ColumnDefinitions.FirstOrDefault(searchFunc);

        if (columnDefinition is null) throw new ColumnDoesNotExistException($"{(string.IsNullOrEmpty(columnName) ? "" : $"columnName: {columnName}")}{(columnId is null ? "" : $"columnId: {columnId}")}\t{(tableID is null ? "" : $"tableId: {tableID}")}{(roleId is null ? "" : $"table roleID: {roleId}")}{(string.IsNullOrEmpty(tableName) ? "" : $"tableName: {tableName}")}.");

        return columnDefinition!;
    }

    internal List<ColumnDefinitionsModel> InternalGetColumns(
        EinDataContext context,
        int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        var tableDefinition = InternalGetTable(context, tableId, roleId, tableName);

        var tableID = tableDefinition.Id;

        var columnDefinitionList = context.ColumnDefinitions.Where(column => column.TableDefinitionsId == tableID).ToList();

        return columnDefinitionList ?? new List<ColumnDefinitionsModel>();
    }

    internal Func<ColumnDefinitionsModel, bool> InternalGetColumnSearchFunc(
        EinDataContext context,
        string? columnName = null, int? columnId = null,
        int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        Func<ColumnDefinitionsModel, bool> columnSearchFunc = (column => false);

        if (tableId is not null || roleId is not null || !string.IsNullOrEmpty(tableName))
        {
            TableDefinitionsModel tableDefinition = InternalGetTable(context, tableId: tableId, roleId: roleId, tableName: tableName);
            int tableID = tableDefinition.Id;

            if (columnId is not null) columnSearchFunc = (column => column.Id == columnId && column.TableDefinitionsId == tableID);
            else columnSearchFunc = (column => column.Name.Equals(columnName) && column.TableDefinitionsId == tableID);
        }
        else
        {
            if (columnId is not null) columnSearchFunc = (column => column.Id == columnId);
        }

        return columnSearchFunc;
    }
}
