namespace EinBotDB.DataAccess;

using EinBotDB.Context;
using EinBotDB.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;

public partial class EinDataAccess
{
    /// <summary>
    /// Add a row to a table.
    /// </summary>
    /// <param name="tableId">The id of the table to add a row to.</param>
    /// <param name="key">The key of the role.</param>
    /// <param name="columnsDataDict">A dictionary with Key:Value of ColumnName:Data</param>
    /// <returns>The row number of the newly insterted row.</returns>
    /// <exception cref="KeyAlreadyPresentInTableException">If the given key is already present in the table.</exception>
    /// <exception cref="InvalidDataException">If one of the given data types doesn't match the data type defined by the column.</exception>
    /// <exception cref="TableDoesNotExistException">If a table with the given tableId does not exist.</exception>"
    public int AddRow(int tableId, string key, Dictionary<string, string>? columnsDataDict = null)
    {
        using var context = _factory.CreateDbContext();

        // Make sure the table actually exists.
        var tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.Id == tableId);

        if (tableDefinition is null) throw new TableDoesNotExistException(tableId);

        // Make sure there's not an entry with the same key already present.
        var hasKey = context.Cells.FirstOrDefault(cell => cell.TableDefinitionsId == tableId && cell.RowKey != null && cell.RowKey.Equals(key));

        if (hasKey is not null) throw new KeyAlreadyPresentInTableException(tableId, key);

        // This will already throw a TableNotFoundException if the table doesn't exist.
        List<ColumnDefinitionsModel> columnsList = GetColumns(tableId);

        int curRow = 0;
        var cells = context.Cells.Where(cell => cell.TableDefinitionsId == tableId).ToList();
        if (cells.Count > 0) curRow = cells.Max(cell => cell.RowNum) + 1;

        columnsDataDict ??= new Dictionary<string, string>();

        foreach(var columnDef in columnsList)
        {
            var cellModel = new CellsModel()
            {
                TableDefinitionsId = tableId,
                ColumnDefinitionsId = columnDef.Id,
                RowKey = key,
                RowNum = curRow,
            };

            if (columnsDataDict.ContainsKey(columnDef.Name))
            {
                var data = columnsDataDict[columnDef.Name];
                DataTypesEnum dataType = (DataTypesEnum)columnDef.DataTypesId;

                if (!IsValidValue(dataType, data)) throw new InvalidDataException($"Column {columnDef.Name} expects {columnDef.DataTypes.Name}.  Received: {data}");
                
                cellModel.Data = data;
            }

            context.Cells.Add(cellModel);
        }

        context.SaveChanges();

        return curRow;
    }

    /// <summary>
    /// Changes the key of a row.
    /// </summary>
    /// <param name="tableId">The id of the table the row is in.</param>
    /// <param name="rowNum">The row number of the row.</param>
    /// <param name="key">The new key to assign.</param>
    /// <returns>True/False of success.</returns>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    public bool ChangeKey(int tableId, int rowNum, string key)
    {
        using var context = _factory.CreateDbContext();

        var tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.Id == tableId);

        if (tableDefinition is null) throw new TableDoesNotExistException(tableId);

        var cells = context.Cells.Where(cell => cell.TableDefinitionsId == tableId && cell.RowNum == rowNum).ToList();

        foreach(var cell in cells)
        {
            cell.RowKey = key;
        }

        context.SaveChanges();

        return true;
    }

    /// <summary>
    /// Removes a row from a table.
    /// </summary>
    /// <param name="tableId">The table to remove the row from.</param>
    /// <param name="rowNum">The number of the row to remove (if null, must provide a key).</param>
    /// <param name="key">The key of the row to remove (if null, must provide a rowNum).</param>
    /// <returns>True/False of success.</returns>
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    public bool DeleteRow(int tableId, int? rowNum = null, string? key = null)
    {
        if (rowNum is null && key is null) return false;

        using var context = _factory.CreateDbContext();

        var tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.Id == tableId);

        if (tableDefinition is null) throw new TableDoesNotExistException(tableId);

        var cells = context.Cells.Where(cell => cell.TableDefinitionsId == tableId);

        if (rowNum is null) cells = cells.Where(cell => !string.IsNullOrEmpty(cell.RowKey) && cell.RowKey.Equals(key));
        else cells = cells.Where(cell => cell.RowNum == rowNum);

        if (cells is null || cells.Count() < 1)
        {
            if (rowNum is null) throw new InvalidKeyException(tableId, key!);
            throw new InvalidRowNumException(tableId, (int)rowNum);
        }

        foreach(var cell in cells)
        {
            context.Cells.Remove(cell);
        }

        context.SaveChanges();

        return true;
    }

    /// <summary>
    /// Updates a row with the given data.
    /// </summary>
    /// <param name="tableId">Id of the table to update the row in.</param>
    /// <param name="columnsDataDict">A dictionary with Key:Value of ColumnName:Data</param>
    /// <param name="rowNum">The row number of the row to update (if null, must provide a key).</param>
    /// <param name="key">The key of the row to update (if null, must provide a rollNum).</param>
    /// <returns>True/False of success.</returns>
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    /// <exception cref="InvalidDataException">If the given data type does not match the defined data type for the column.</exception>
    public bool UpdateRow(int tableId, Dictionary<string, string> columnsDataDict, int? rowNum = null, string? key = null)
    {
        if (rowNum is null && key is null) return false;

        using var context = _factory.CreateDbContext();

        var tableDefinition = context.TableDefinitions.FirstOrDefault(table => table.Id == tableId);

        if (tableDefinition == null) throw new TableDoesNotExistException(tableId);

        List<CellsModel> cells;

        if (rowNum is null) cells = context.Cells.Where(cell =>
                                                    cell.TableDefinitionsId == tableId
                                                    && !string.IsNullOrEmpty(cell.RowKey)
                                                    && cell.RowKey.Equals(key))
                                                 .ToList();

        else cells = context.Cells.Where(cell => cell.TableDefinitionsId == tableId && cell.RowNum == rowNum).ToList();

        var columnsList = context.ColumnDefinitions.Where(column => column.TableDefinitionsId == tableDefinition.Id);

        // TODO
        foreach(var colName in columnsDataDict.Keys)
        {

        }

        foreach(var cell in cells)
        {
            var column = columnsList.FirstOrDefault(column => column.Id == cell.ColumnDefinitionsId);

            if (column is null) continue;

            var columnDataType = (DataTypesEnum)column.DataTypesId;

            if (columnsDataDict.ContainsKey(column.Name))
            {
                var data = columnsDataDict[column.Name];

                if (!IsValidValue(columnDataType, data)) throw new InvalidDataException($"{column.Name} expects {columnDataType}.  Received: {data}"); ;

                cell.Data = data;
            }
        }

        context.SaveChanges();

        return true;
    }

    /// <summary>
    /// Retrieves the data of the cell in the given column of the given table.
    /// </summary>
    /// <param name="tableName">The name of the table the cell is in.</param>
    /// <param name="columnName"></param>
    /// <param name="dataType">OUT the data type associated with the cell.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <returns>The data in the given cell.</returns>
    /// <exception cref="InvalidDataException">If unable to determine the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public string? GetCellValue(string tableName, string columnName, out DataTypesEnum? dataType, int? rowNum = null, string? rowKey = null)
    {
        return GetCellValueHelper(columnName, out dataType, tableName: tableName, rowNum: rowNum, rowKey: rowKey);
    }

    /// <summary>
    /// Retrieves the data of the cell in the given column of the given table.
    /// </summary>
    /// <param name="roleId">The id of the role associated with the table the cell is in.</param>
    /// <param name="columnName"></param>
    /// <param name="dataType">OUT the data type associated with the cell.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <returns>The data in the given cell.</returns>
    /// <exception cref="InvalidDataException">If unable to determine the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public string? GetCellValue(ulong roleId, string columnName, out DataTypesEnum? dataType, int? rowNum = null, string? rowKey = null)
    {
        return GetCellValueHelper(columnName, out dataType, roleId: roleId, rowNum: rowNum, rowKey: rowKey);
    }

    /// <summary>
    /// Retrieves the data of the cell in the given column of the given table.
    /// </summary>
    /// <param name="tableId">The id of the table the cell is in.</param>
    /// <param name="columnName"></param>
    /// <param name="dataType">OUT the data type associated with the cell.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <returns>The data in the given cell.</returns>
    /// <exception cref="InvalidDataException">If unable to determine the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public string? GetCellValue(int tableId, string columnName, out DataTypesEnum? dataType, int? rowNum = null, string? rowKey = null)
    {
        return GetCellValueHelper(columnName, out dataType, tableId: tableId, rowNum: rowNum, rowKey: rowKey);
    }

    /// <summary>
    /// Sets the value of the cell in the given table and column.
    /// </summary>
    /// <param name="tableName">The name of the table the cell is in.</param>
    /// <param name="columnName">The name of the column the cell is in.</param>
    /// <param name="data">The data to set the cell to.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <exception cref="InvalidDataException">If the given modifier does not match the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public void SetCellValue(string tableName, string columnName, string data, int? rowNum = null, string? rowKey = null)
    {
        SetCellValueHelper(data, columnName, tableName: tableName, rowNum: rowNum, rowKey: rowKey);
    }

    /// <summary>
    /// Sets the value of the cell in the given table and column.
    /// </summary>
    /// <param name="roleId">The id of the role associated with the table the cell is in.</param>
    /// <param name="columnName">The name of the column the cell is in.</param>
    /// <param name="data">The data to set the cell to.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <exception cref="InvalidDataException">If the given modifier does not match the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public void SetCellValue(ulong roleId, string columnName, string data, int? rowNum = null, string? rowKey = null)
    {
        SetCellValueHelper(data, columnName, roleId: roleId, rowNum: rowNum, rowKey: rowKey);
    }

    /// <summary>
    /// Sets the value of the cell in the given table and column.
    /// </summary>
    /// <param name="tableId">The id of the table the cell is in.</param>
    /// <param name="columnName">The name of the column the cell is in.</param>
    /// <param name="data">The data to set the cell to.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <exception cref="InvalidDataException">If the given modifier does not match the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public void SetCellValue(int tableId, string columnName, string data, int? rowNum = null, string? rowKey = null)
    {
        SetCellValueHelper(data, columnName, tableId: tableId, rowNum: rowNum, rowKey: rowKey);
    }

    /// <summary>
    /// Modifies the value of the cell.  If numeric type, it will add or subtract the given modifier.  If text, it will simply set the data to the modifier.
    /// </summary>
    /// <param name="roleId">The id of the role associated with the table the cell is in.</param>
    /// <param name="columnName">The name of the column the cell is in.</param>
    /// <param name="modifier">The modifier to apply to the data.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">If the given modifier does not match the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public bool ModifyCellValue(ulong roleId, string columnName, string modifier, int? rowNum = null, string? rowKey = null)
    {
        return ModifyCellValueHelper(modifier, columnName, roleId: roleId, rowNum: rowNum, rowKey: rowKey);
    }

    /// <summary>
    /// Modifies the value of the cell.  If numeric type, it will add or subtract the given modifier.  If text, it will simply set the data to the modifier.
    /// </summary>
    /// <param name="tableId">The id of the table the cell is in.</param>
    /// <param name="columnName">The name of the column the cell is in.</param>
    /// <param name="modifier">The modifier to apply to the data.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">If the given modifier does not match the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public bool ModifyCellValue(int tableId, string columnName, string modifier, int? rowNum = null, string? rowKey = null)
    {
        return ModifyCellValueHelper(modifier, columnName, tableId: tableId, rowNum: rowNum, rowKey: rowKey);
    }

    /// <summary>
    /// Modifies the value of the cell.  If numeric type, it will add or subtract the given modifier.  If text, it will simply set the data to the modifier.
    /// </summary>
    /// <param name="tableName">The name of the table the cell is in.</param>
    /// <param name="columnName">The name of the column the cell is in.</param>
    /// <param name="modifier">The modifier to apply to the data.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">If the given modifier does not match the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public bool ModifyCellValue(string tableName, string columnName, string modifier, int? rowNum = null, string? rowKey = null)
    {
        return ModifyCellValueHelper(modifier, columnName, tableName: tableName, rowNum: rowNum, rowKey: rowKey);
    }

    /// <summary>
    /// Helper func for getting the value of a cell.
    /// </summary>
    /// <param name="columnName"></param>
    /// <param name="dataType"></param>
    /// <param name="tableId"></param>
    /// <param name="roleId"></param>
    /// <param name="tableName"></param>
    /// <param name="rowNum"></param>
    /// <param name="rowKey"></param>
    /// <returns></returns>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given column does not exist on the table.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cell in the given column.</exception>
    /// <exception cref="InvalidDataException">If it's unable to determine the datatype of the cell.</exception>
    private string? GetCellValueHelper(string columnName, out DataTypesEnum? dataType, int? tableId = null, ulong? roleId = null, string? tableName = null, int? rowNum = null, string? rowKey = null)
    {
        dataType = null;
        if (string.IsNullOrEmpty(columnName)) throw new ColumnDoesNotExistException("Column name cannot be null.");

        using var context = _factory.CreateDbContext();

        if (!GetCellColumnTableType(context, columnName,
            out CellsModel cellsModel, out TableDefinitionsModel tableDefinition, out ColumnDefinitionsModel columnDefinition, out DataTypesEnum? dType,
            tableName, tableId, roleId, rowNum, rowKey)) throw new CellDoesNotExistException();

        if (dType is null) throw new InvalidDataException("Unable to parse column data type.");

        dataType = (DataTypesEnum)dType;

        return cellsModel.Data;
    }

    /// <summary>
    /// Helper func for setting a cell value.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="columnName"></param>
    /// <param name="tableId"></param>
    /// <param name="roleId"></param>
    /// <param name="tableName"></param>
    /// <param name="rowNum"></param>
    /// <param name="rowKey"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">If the given data does not match the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    private void SetCellValueHelper(string data, string columnName, int? tableId = null, ulong? roleId = null, string? tableName = null, int? rowNum = null, string? rowKey = null)
    {
        if (string.IsNullOrEmpty(columnName)) throw new ColumnDoesNotExistException("Column names cannot be null.");

        using var context = _factory.CreateDbContext();

        if (!GetCellColumnTableType(context, columnName,
                out CellsModel cellsModel, out TableDefinitionsModel tableDefinition, out ColumnDefinitionsModel columnDefinition, out DataTypesEnum? dType,
                tableName, tableId, roleId, rowNum, rowKey))
        {
            throw new CellDoesNotExistException();
        }

        if (dType is null) throw new InvalidDataException($"Unable to discern data type of cell.");

        DataTypesEnum dataType = (DataTypesEnum)dType;

        if (!IsValidValue(dataType, data)) throw new InvalidDataException($"Invalid data type for table {tableDefinition.Id}, column {columnDefinition.Id}.  Was expecting {dataType}, received {data}.");

        cellsModel.Data = data;

        context.SaveChanges();
    }

    /// <summary>
    /// Helper func for modifying cell values.
    /// </summary>
    /// <param name="modifier"></param>
    /// <param name="columnName"></param>
    /// <param name="tableId"></param>
    /// <param name="roleId"></param>
    /// <param name="tableName"></param>
    /// <param name="rowNum"></param>
    /// <param name="rowKey"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">If the given modifier does not match the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    private bool ModifyCellValueHelper(string modifier, string columnName, int? tableId = null, ulong? roleId = null, string? tableName = null, int? rowNum = null, string? rowKey = null)
    {
        if (string.IsNullOrEmpty(modifier)) return false;
        if (string.IsNullOrEmpty(columnName)) return false;

        using var context = _factory.CreateDbContext();

        if (!GetCellColumnTableType(context, columnName,
            out CellsModel cellsModel, out TableDefinitionsModel tableDefinition, out ColumnDefinitionsModel columnDefinition, out DataTypesEnum? dType,
            tableName, tableId, roleId, rowNum, rowKey)) return false;

        if (dType is null) return false;

        DataTypesEnum dataType = (DataTypesEnum)dType;

        if (!IsValidValue(dataType, modifier)) throw new InvalidDataException($"Table id {tableDefinition!.Id} column {columnDefinition!.Id} is type {dataType}.  Input: {modifier}");

        var data = cellsModel!.Data;

        switch (dataType)
        {
            case DataTypesEnum.Int:
            case DataTypesEnum.ListInt:
                int intVal;
                int intModifier;

                if (string.IsNullOrEmpty(data)) data = "0";
                if (string.IsNullOrEmpty(modifier)) modifier = "0";

                if (!int.TryParse(data, out intVal) || !int.TryParse(modifier, out intModifier)) return false;

                cellsModel.Data = (intVal + intModifier).ToString();
                break;
            case DataTypesEnum.Decimal:
            case DataTypesEnum.ListDecimal:
                double decimalVal;
                double decimalModifier;

                if (string.IsNullOrEmpty(data)) data = "0";
                if (string.IsNullOrEmpty (modifier)) modifier = "0";

                if (!double.TryParse(data, out decimalVal) || !double.TryParse(modifier, out decimalModifier)) return false;

                cellsModel.Data = (decimalVal + decimalModifier).ToString();
                break;
            case DataTypesEnum.UserId:
            case DataTypesEnum.ListUserId:
            case DataTypesEnum.GuildId:
            case DataTypesEnum.ListGuildId:
            case DataTypesEnum.ChannelId:
            case DataTypesEnum.ListChannelId:
                ulong ulongVal;
                ulong ulongModifier;

                if (string.IsNullOrEmpty(data)) data = "0";
                if (string.IsNullOrEmpty(modifier)) modifier = "0";

                if (!ulong.TryParse(data, out ulongVal) || !ulong.TryParse(modifier, out ulongModifier)) return false;

                cellsModel.Data = (ulongVal + ulongModifier).ToString();
                break;
            default:
                cellsModel.Data = modifier;
                break;
        }

        context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Grabs a cell, its column, table, and data type given a column name, (role ID / table Name / table ID).
    /// </summary>
    /// <param name="context">DB Context we're operating in.</param>
    /// <param name="columnName">The name of the column the cell is in.</param>
    /// <param name="cellsModel">OUT CellsModel object.</param>
    /// <param name="tableDefinition">OUT TableDefinitionModel object.</param>
    /// <param name="columnDefinition">OUT ColumnDefinitionModel object.</param>
    /// <param name="dataType">OUT DataTypesEnum object.</param>
    /// <param name="tableName">Null or the name of the table. Must not be null if tableId and roleId are null.</param>
    /// <param name="tableId">Null or the id of the table.  Must not be null if tableName and roleId are null.</param>
    /// <param name="roleId">Null or the role id associated with the table.  Must not be null if tableId and tableName are null.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Must not be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the cell's row.  Must not be null of rowNum is null.</param>
    /// <returns></returns>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    private bool GetCellColumnTableType(EinDataContext context, string columnName, out CellsModel? cellsModel,
                        out TableDefinitionsModel? tableDefinition, out ColumnDefinitionsModel? columnDefinition,
                        out DataTypesEnum? dataType, string? tableName = null,
                        int? tableId = null, ulong? roleId = null,
                        int? rowNum = null, string? rowKey = null)
    {
        cellsModel = null;
        tableDefinition = null;
        columnDefinition = null;
        dataType = null;

        if (rowNum is null && string.IsNullOrEmpty(rowKey)) return false;
        if (tableId is null && roleId is null && string.IsNullOrEmpty(tableName)) return false;

        Func<TableDefinitionsModel, bool> searchFunc = (table => table.Id == tableId);

        if (roleId is not null)
        {
            searchFunc = (table => table.RoleId == roleId);
        }
        else if (!string.IsNullOrEmpty(tableName))
        {
            searchFunc = (table => table.Name.Equals(tableName));

        }

        tableDefinition = context.TableDefinitions.FirstOrDefault(searchFunc);

        if (tableDefinition is null) throw new TableDoesNotExistException();

        tableId = tableDefinition.Id;

        columnDefinition = context.ColumnDefinitions.FirstOrDefault(col => col.TableDefinitionsId == tableId && col.Name.Equals(columnName));
        
        if (columnDefinition is null) throw new ColumnDoesNotExistException(tableDefinition.Name, columnName);

        var columnDefinitionId = columnDefinition.Id;
        dataType = (DataTypesEnum)columnDefinition.DataTypesId;

        Func<CellsModel, bool> cellSearchFunc = (cell => cell.TableDefinitionsId == tableId
                                                        && cell.ColumnDefinitionsId == columnDefinitionId
                                                        && cell.RowKey is not null
                                                        && cell.RowKey.Equals(rowKey));

        if (rowNum is not null)
        {
            cellSearchFunc = (cell => cell.TableDefinitionsId == tableId
                                        && cell.ColumnDefinitionsId == columnDefinitionId
                                        && cell.RowNum == rowNum);
        }

        cellsModel = context.Cells.FirstOrDefault(cellSearchFunc);

        if (cellsModel is null) { if (rowNum is not null) throw new CellDoesNotExistException(tableId, columnName, rowNum); else throw new CellDoesNotExistException(tableId, columnName, rowKey); }

        return true;
    } 


    /// <summary>
    /// Checks if the given data matches the given data type.
    /// </summary>
    /// <param name="dataType">The data type to check the data against.</param>
    /// <param name="data">The data to check against the data type.</param>
    /// <returns>True if data matches the dataType, false otherwise.</returns>
    public bool IsValidValue(DataTypesEnum dataType, string data)
    {
        switch(dataType)
        {
            case DataTypesEnum.Text:
            case DataTypesEnum.ListText:
                return true;
            case DataTypesEnum.Int:
            case DataTypesEnum.ListInt:
                return int.TryParse(data, out _);
            case DataTypesEnum.Decimal:
            case DataTypesEnum.ListDecimal:
                return double.TryParse(data, out _);
            case DataTypesEnum.UserId:
            case DataTypesEnum.ListUserId:
            case DataTypesEnum.GuildId:
            case DataTypesEnum.ListGuildId:
            case DataTypesEnum.ChannelId:
            case DataTypesEnum.ListChannelId:
                return ulong.TryParse(data, out _);
            default: return false;
        }
    }
}
