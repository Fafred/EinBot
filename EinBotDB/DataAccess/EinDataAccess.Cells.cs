namespace EinBotDB.DataAccess;

using EinBotDB.Context;
using EinBotDB.Models;
using System;

public partial class EinDataAccess
{
    /*****
     * 
     * This partial class implementation handles all cell related data access operations.
     * 
     ****/

    /// <summary>
    /// Adds or updates a "row" to the given table.  If no dataDict is supplied, or a column isn't present in the dataDict, the default value for the column will be an empty string.
    /// </summary>
    /// <param name="rowKey">The key of the row.  This must be unique key for the table.</param>
    /// <param name="dataDict">Optional Dictionary<string, string>, where the key is the name of the column and the value is the data to put into it.</param>
    /// <param name="tableId">NULL or the id of the table to add/update a row in.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table to add/update a row in.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table to add/update a row in.  If null, then either roleId or tableId cannot be null.</param>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="InvalidKeyException">If the key is null.</exception>
    /// <exception cref="KeyAlreadyPresentInTableException">If the key is already present in the table.</exception>"
    /// <exception cref="InvalidDataException">If the data given in one of the values in the dataDict does not match the column data type.</exception>
    /// <exception cref="InvalidKeyException"></exception>
    public void AddOrUpdateRow(
        string rowKey,
        Dictionary<string, string>? dataDict = null,
        int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        if (string.IsNullOrEmpty(rowKey)) throw new InvalidKeyException("Key cannot be null.");

        using var context = _factory.CreateDbContext();

        var tableDefinition = InternalGetTable(context, tableId: tableId, roleId: roleId, tableName: tableName);
        var tableID = tableDefinition.Id;

        if (dataDict is null) dataDict = new();

        if ((context.Cells.FirstOrDefault(cell => cell.TableDefinitionsId == tableID && !string.IsNullOrEmpty(cell.RowKey) && cell.RowKey.Equals(rowKey))) is not null)
        {
            UpdateRow(dataDict, tableId: tableID, rowKey: rowKey);
        }
        else
        {
            AddRow(rowKey, dataDict, tableId: tableID);
        }
    }

    /// <summary>
    /// Adds a "row" to the given table.  If no dataDict is supplied, or a column isn't present in the dataDict, the default value for the column will be an empty string.
    /// </summary>
    /// <param name="rowKey">The key to give to the row.  This must be unique key for the table.</param>
    /// <param name="dataDict">Optional Dictionary<string, string>, where the key is the name of the column and the value is the data to put into it.</param>
    /// <param name="tableId">NULL or the id of the table to make a row in.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table to make a row in.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table to make a row in.  If null, then either roleId or tableId cannot be null.</param>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="InvalidKeyException">If the key is null.</exception>
    /// <exception cref="KeyAlreadyPresentInTableException">If the key is already present in the table.</exception>"
    /// <exception cref="InvalidDataException">If the data given in one of the values in the dataDict does not match the column data type.</exception>
    public void AddRow(
        string rowKey,
        Dictionary<string, string>? dataDict = null,
        int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        if (string.IsNullOrEmpty(rowKey)) throw new InvalidKeyException("Key cannot be null.");

        using var context = _factory.CreateDbContext();

        // Make sure the table exists, and get its ID.
        var tableDefinition = InternalGetTable(context, tableId, roleId, tableName);
        int tableID = tableDefinition.Id;

        // Grab the columns associated with this table.
        var columnsList = InternalGetColumns(context, tableId, roleId, tableName);

        // If there are no columns, there's no row to add.
        if (columnsList is null || columnsList.Count < 1) return;

        // Check if the key is already present, and calculate the insert row if not.
        var existingCells = context.Cells.Where(cell => cell.TableDefinitionsId == tableId);

        if (existingCells.FirstOrDefault(cell => !string.IsNullOrEmpty(cell.RowKey) && cell.RowKey.Equals(rowKey)) is not null) throw new KeyAlreadyPresentInTableException((int)tableId!, rowKey);

        int insertRow = 1;

        // Calculate the new row number based off the number of "rows" for this table which are already in the cells table.
        if (existingCells is not null && existingCells.Any()) insertRow = existingCells.Max(cell => cell.RowNum) + 1;

        // Now we can create the row.
        List<CellsModel> cells = new();

        dataDict ??= new Dictionary<string, string>();

        foreach (var column in columnsList)
        {
            string data = "";

            if (dataDict.ContainsKey(column.Name)) data = dataDict[column.Name];

            var dataType = (DataTypesEnum)column.DataTypesId;

            if (!IsValidValue(dataType, data)) throw new InvalidDataException($"{column.Name} expects type {dataType} received {data}.");

            var cell = new CellsModel
            {
                TableDefinitionsId = tableID,
                ColumnDefinitionsId = column.Id,
                RowKey = rowKey,
                RowNum = insertRow,
                Data = data,
            };

            cells.Add(cell);
        }

        context.Cells.AddRange(cells);
        context.SaveChanges();
    }

    /// <summary>
    /// Changes the key of a given row in a table.
    /// </summary>
    /// <param name="newKey">The new key for the given row in the table.</param>
    /// <param name="tableId">NULL or the id of the table to change the row key in.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table to change the row key in.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table to change the row key in.  If null, then either roleId or tableId cannot be null.</param>
    /// <param name="rowNum">NULL or the row num of the row to change the key of.  If null, rowKEY cannot be null.</param>
    /// <param name="rowKey">NULL or the key of the row to change the key of.  If null, rowNum cannot be null.</param>
    /// <returns></returns>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="InvalidKeyException">If the new key is null, or if the rowKey and rowNum are both null.</exception>
    /// <exception cref="CellDoesNotExistException">If there are no cells with the given rowKey and rowNum.</exception>
    /// <exception cref="KeyAlreadyPresentInTableException">If the table already has cells with the newKey key.</exception>
    public string ChangeKey(
        string newKey,
        int? tableId = null, ulong? roleId = null, string? tableName = null,
        int? rowNum = null, string? rowKey = null)
    {
        if (string.IsNullOrEmpty(newKey)) throw new InvalidKeyException(newKey);
        if (string.IsNullOrEmpty(rowKey) && rowNum is null) throw new InvalidKeyException("rowNum and rowKey can't both be null.");

        using var context = _factory.CreateDbContext();

        var tableDefinition = InternalGetTable(context, tableId, roleId, tableName);
        int tableID = tableDefinition.Id;

        var tableCells = InternalGetCells(context, tableId: tableID);

        // Make sure key isn't already present in the table's cells.
        var keyTestCell = tableCells.FirstOrDefault(cell => !string.IsNullOrEmpty(cell.RowKey) && cell.RowKey.Equals(newKey));

        if (keyTestCell is not null) throw new KeyAlreadyPresentInTableException(tableID, newKey);

        // Get the cells associated with the given rowNum or rowKey.
        Func<CellsModel, bool> searchFunc;

        if (rowNum is not null) searchFunc = (cell => cell.RowNum == rowNum);
        else searchFunc = (cell => !string.IsNullOrEmpty(cell.RowKey) && !cell.RowKey.Equals(rowKey));

        var cellList = tableCells.Where(searchFunc);

        // Make sure we actually have cells.
        if (cellList is null || !cellList.Any()) throw new CellDoesNotExistException();

        // Set the new key.
        string oldKey = cellList.First().RowKey ?? "";

        foreach (var cell in cellList)
        {
            cell.RowKey = newKey;
        }

        context.SaveChanges();

        return oldKey;
    }

    /// <summary>
    /// Deletes a given row from the table.
    /// </summary>
    /// <param name="tableId">NULL or the id of the table to delete the row in.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table to delete the row in.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table to delete the row in.  If null, then either roleId or tableId cannot be null.</param>
    /// <param name="rowNum">NULL or the row num of the row to delete the row in.  If null, rowKEY cannot be null.</param>
    /// <param name="rowKey">NULL or the key of the row to delete the row in.  If null, rowNum cannot be null.</param>
    /// <exception cref="InvalidKeyException"></exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    public void DeleteRow(
        int? tableId = null, ulong? roleId = null, string? tableName = null,
        int? rowNum = null, string? rowKey = null)
    {
        if (rowNum is null && string.IsNullOrEmpty(rowKey)) throw new InvalidKeyException("RowNum and RowKey can't both be null.");

        using var context = _factory.CreateDbContext();

        var tableDefinition = InternalGetTable(context, tableId, roleId, tableName);
        int tableID = tableDefinition.Id;

        var cellsList = InternalGetCells(context, tableId: tableId, rowNum: rowNum, rowKey: rowKey);

        foreach (var cell in cellsList)
        {
            context.Cells.Remove(cell);
        }

        context.SaveChanges();
    }

    /// <summary>
    /// Retrieves the value of a cell from the given table, column, and row.
    /// </summary>
    /// <param name="tableId">NULL or the id of the table to get the cell value of.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table get the cell value of.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table get the cell value of.  If null, then either roleId or tableId cannot be null.</param>
    /// <param name="columnId">NULL or the column id of the cell to get the value of.  If null, then columnName cannot be null.</param>
    /// <param name="columnName">NULL or the column name of the cell to get the value of.  If null, then columnId cannot be null.</param>
    /// <param name="rowNum">NULL or the row num of the row to get the cell value of.  If null, rowKEY cannot be null.</param>
    /// <param name="rowKey">NULL or the key of the row to get the cell value of.  If null, rowNum cannot be null.</param>
    /// <returns>A string of the cell's value.  If the value is null, then it will return an empty string.</returns>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given column does not exist.</exception>
    /// <exception cref="CellDoesNotExistException">If the given cell does not exist.</exception>
    public string GetCellValue(
        int? tableId = null, ulong? roleId = null, string? tableName = null,
        int? columnId = null, string? columnName = null,
        int? rowNum = null, string? rowKey = null)
    {
        CellGateKeep(tableId, roleId, tableName, columnId, columnName, rowNum, rowKey);

        using var context = _factory.CreateDbContext();

        var cell = InternalGetCell(context, columnName, columnId, tableId, roleId, tableName, rowNum, rowKey);

        if (cell is null) throw new CellDoesNotExistException();

        return cell.Data ?? "";
    }

    /// <summary>
    /// Checks if the given data matches the given data type.
    /// </summary>
    /// <param name="dataType">The data type to check the data against.</param>
    /// <param name="data">The data to check against the data type.</param>
    /// <returns>True if data matches the dataType, false otherwise.</returns>
    public bool IsValidValue(DataTypesEnum dataType, string data)
    {
        if (string.IsNullOrEmpty(data)) return true;

        switch (dataType)
        {
            case DataTypesEnum.Text:
            //case DataTypesEnum.ListText:
                return true;
            case DataTypesEnum.Int:
            //case DataTypesEnum.ListInt:
                return int.TryParse(data, out _);
            case DataTypesEnum.Decimal:
            //case DataTypesEnum.ListDecimal:
                return double.TryParse(data, out _);
            case DataTypesEnum.UserId:
            //case DataTypesEnum.ListUserId:
            case DataTypesEnum.GuildId:
            //case DataTypesEnum.ListGuildId:
            case DataTypesEnum.ChannelId:
            //case DataTypesEnum.ListChannelId:
                return ulong.TryParse(data, out _);
            default: return false;
        }
    }

    /// <summary>
    /// Modifies the value in a cell.  If the cell data type is a string, it will replace it.  If numeric, it will add or subtract.
    /// </summary>
    /// <param name="modifier">Value to modify the cell data by.</param>
    /// <param name="tableId">NULL or the id of the table to modify the cell value of.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table modify the cell value of.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table modify the cell value of.  If null, then either roleId or tableId cannot be null.</param>
    /// <param name="columnId">NULL or the column id of the cell to modify the value of.  If null, then columnName cannot be null.</param>
    /// <param name="columnName">NULL or the column name of the modify to get the value of.  If null, then columnId cannot be null.</param>
    /// <param name="rowNum">NULL or the row num of the row to get the modify value of.  If null, rowKEY cannot be null.</param>
    /// <param name="rowKey">NULL or the key of the row to get the modify value of.  If null, rowNum cannot be null.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">If the given data type does not match the cell's data type (ie, text when the cell is a numeric).</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given column does not exist.</exception>
    /// <exception cref="InvalidKeyException">If rowNum and rowKey are null</exception>"
    /// <exception cref="CellDoesNotExistException">If there are no cells with the given rowNum or rowKey.</exception>"
    public bool ModifyCellValue(
        string modifier,
        int? tableId = null, ulong? roleId = null, string? tableName = null,
        int? columnId = null, string? columnName = null,
        int? rowNum = null, string? rowKey = null)
    {
        CellGateKeep(tableId, roleId, tableName, columnId, columnName, rowNum, rowKey);

        using var context = _factory.CreateDbContext();

        var columnDefinition = InternalGetColumn(context, columnName, columnId, tableId, roleId, tableName);

        var tableID = columnDefinition.TableDefinitionsId;
        var columnID = columnDefinition.Id;
        var dataType = (DataTypesEnum)columnDefinition.DataTypesId;

        if (!IsValidValue(dataType, modifier)) throw new InvalidDataException($"Cell expects {dataType}.  Received {modifier}.");

        var cellsModel = InternalGetCell(context, tableId: tableID, columnId: columnID, rowNum: rowNum, rowKey: rowKey);
        var data = cellsModel.Data;

        switch (dataType)
        {
            case DataTypesEnum.Int:
            //case DataTypesEnum.ListInt:
                int intVal;
                int intModifier;

                if (string.IsNullOrEmpty(data)) data = "0";
                if (string.IsNullOrEmpty(modifier)) modifier = "0";

                if (!int.TryParse(data, out intVal) || !int.TryParse(modifier, out intModifier)) return false;

                cellsModel.Data = (intVal + intModifier).ToString();
                break;
            case DataTypesEnum.Decimal:
            //case DataTypesEnum.ListDecimal:
                double decimalVal;
                double decimalModifier;

                if (string.IsNullOrEmpty(data)) data = "0";
                if (string.IsNullOrEmpty(modifier)) modifier = "0";

                if (!double.TryParse(data, out decimalVal) || !double.TryParse(modifier, out decimalModifier)) return false;

                cellsModel.Data = (decimalVal + decimalModifier).ToString();
                break;
            case DataTypesEnum.UserId:
            //case DataTypesEnum.ListUserId:
            case DataTypesEnum.GuildId:
            //case DataTypesEnum.ListGuildId:
            case DataTypesEnum.ChannelId:
            //case DataTypesEnum.ListChannelId:
            case DataTypesEnum.RoleId:
            //case DataTypesEnum.ListRoleId:
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
    /// Sets the value in a cell.
    /// </summary>
    /// <param name="newValue">Value to set the cell data to.</param>
    /// <param name="tableId">NULL or the id of the table to set the cell value of.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table set the cell value of.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table set the cell value of.  If null, then either roleId or tableId cannot be null.</param>
    /// <param name="columnId">NULL or the column id of the cell to set the value of.  If null, then columnName cannot be null.</param>
    /// <param name="columnName">NULL or the column name of the set to get the value of.  If null, then columnId cannot be null.</param>
    /// <param name="rowNum">NULL or the row num of the row to get the set value of.  If null, rowKEY cannot be null.</param>
    /// <param name="rowKey">NULL or the key of the row to get the set value of.  If null, rowNum cannot be null.</param>
    /// <returns>A string with the old value.</returns>
    /// <exception cref="InvalidDataException">If the given data type does not match the cell's data type (ie, text when the cell is a numeric).</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given column does not exist.</exception>
    /// <exception cref="InvalidKeyException">If rowNum and rowKey are null</exception>"
    /// <exception cref="CellDoesNotExistException">If there are no cells with the given rowNum or rowKey.</exception>"
    public string SetCellValue(
        string newValue,
        int? tableId = null, ulong? roleId = null, string? tableName = null,
        int? columnId = null, string? columnName = null,
        int? rowNum = null, string? rowKey = null)
    {
        CellGateKeep(tableId, roleId, tableName, columnId, columnName, rowNum, rowKey);

        using var context = _factory.CreateDbContext();

        var columnDefinition = InternalGetColumn(context, columnName, columnId, tableId, roleId, tableName);

        var tableID = columnDefinition.TableDefinitionsId;
        var columnID = columnDefinition.Id;
        var dataType = (DataTypesEnum)columnDefinition.DataTypesId;

        var cell = InternalGetCell(context, columnId: columnID, tableId: tableID, rowNum: rowNum, rowKey: rowKey);

        if (cell is null) throw new CellDoesNotExistException();

        if (!IsValidValue(dataType, newValue)) throw new InvalidDataException($"Cell expects {dataType}.  Received: {newValue}.");

        var oldValue = cell.Data ?? "";

        cell.Data = newValue;

        context.SaveChanges();

        return oldValue;
    }

    /// <summary>
    /// Updates multiple cells in a row based off the provided updateData dict.  Key:Value should be ColumnName:Data.
    /// </summary>
    /// <param name="updateData"></param>
    /// <param name="tableId">NULL or the id of the table to update the cells of.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table to update the cells of.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table to update the cells.  If null, then either roleId or tableId cannot be null.</param>
    /// <param name="rowNum">NULL or the row num of the row to update the cells of.  If null, rowKEY cannot be null.</param>
    /// <param name="rowKey">NULL or the key of the row to update the cells of.  If null, rowNum cannot be null.</param>
    /// <exception cref="InvalidDataException">If the given data type does not match the cell's data type (ie, text when the cell is a numeric).</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given column does not exist.</exception>
    /// <exception cref="InvalidKeyException">If rowNum and rowKey are null</exception>"
    /// <exception cref="CellDoesNotExistException">If there are no cells with the given rowNum or rowKey.</exception>"
    /// <exception cref="KeyNotFoundException">If the given keys do not exist in any rows of the table.</exception>
    public void UpdateRow(
        Dictionary<string, string> updateData,
        int? tableId = null, ulong? roleId = null, string? tableName = null,
        int? rowNum = null, string? rowKey = null)
    {
        // Validate rowNum/rowKey
        if (rowNum is null && string.IsNullOrEmpty(rowKey)) throw new InvalidKeyException("RowNum and RowKey cannot both be null.");

        // Don't bother if the dictionary contains no items.
        if (updateData.Count < 1) return;

        using var context = _factory.CreateDbContext();

        // Make sure the table exists.
        var tableDefinition = InternalGetTable(context, tableId: tableId, roleId: roleId, tableName: tableName);
        var tableID = tableDefinition.Id;

        // Grab the columns and the cells.
        var columnsList = InternalGetColumns(context, tableId: tableID);
        var cellsList = InternalGetCells(context, tableId: tableID, rowNum: rowNum, rowKey: rowKey);

        if (!cellsList.Any()) throw new KeyNotFoundException($"No row with rowNum: {rowNum} or rowKey: {rowKey}.");

        var keyList = updateData.Keys.ToList();

        List<CellsModel> cellsToUpdate = new();

        foreach (var key in keyList)
        {
            // Make sure the column from updateData exists, and that the data matches the data type of the actual column.
            var columnDefinition = columnsList.FirstOrDefault(column => column.Name.Equals(key));

            if (columnDefinition is null) continue; //throw new ColumnDoesNotExistException($"No column with the name {key} in table id {tableID}.");

            var dataType = (DataTypesEnum)columnDefinition.DataTypesId;
            var data = updateData[key];

            if (!IsValidValue(dataType, data)) throw new InvalidDataException($"Column {columnDefinition.Id} was expecting {dataType}.  Received {data}.");

            var cell = cellsList.FirstOrDefault(cell => cell.ColumnDefinitionsId == columnDefinition.Id);

            if (cell is null) throw new CellDoesNotExistException($"No cell for column {columnDefinition.Id} on table {tableID}.");

            cell.Data = data;

            cellsToUpdate.Add(cell);
        }

        context.Cells.UpdateRange(cellsToUpdate);

        context.SaveChanges();
    }

    internal CellsModel InternalGetCell(
        EinDataContext context,
        string? columnName = null, int? columnId = null,
        int? tableId = null, ulong? roleId = null, string? tableName = null,
        int? rowNum = null, string? rowKey = null)
    {
        if (rowNum is null && rowKey is null) throw new CellDoesNotExistException();

        var columnDefiniton = InternalGetColumn(context, columnName, columnId, tableId, roleId, tableName);

        var tableID = columnDefiniton.TableDefinitionsId;
        var columnID = columnDefiniton.Id;

        Func<CellsModel, bool> searchFunc;

        if (rowNum is not null) searchFunc = (cell => cell.TableDefinitionsId == tableId && cell.ColumnDefinitionsId == columnId && cell.RowNum == rowNum);
        else searchFunc = (cell => cell.TableDefinitionsId == tableId && cell.ColumnDefinitionsId == columnId && !string.IsNullOrEmpty(cell.RowKey) && cell.RowKey!.Equals(rowKey));

        var cell = context.Cells.FirstOrDefault(searchFunc);

        if (cell is null) throw new CellDoesNotExistException(tableId, columnDefiniton.Name, $"{(rowNum is null ? (rowKey is null ? "NULL" : rowKey) : rowNum)}");

        return cell;
    }

    /// <summary>
    /// Gets the cells which match the given criteria (ie, just tableid returns all cells, tableid and rowid return just the row, tableid and columnid return all that column's cells in the table.  &c
    /// </summary>
    /// <param name="context"></param>
    /// <param name="columnName"></param>
    /// <param name="columnId"></param>
    /// <param name="tableId"></param>
    /// <param name="roleId"></param>
    /// <param name="tableName"></param>
    /// <param name="rowNum"></param>
    /// <param name="rowKey"></param>
    /// <returns></returns>
    internal List<CellsModel> InternalGetCells(
        EinDataContext context,
        string? columnName = null, int? columnId = null,
        int? tableId = null, ulong? roleId = null, string? tableName = null,
        int? rowNum = null, string? rowKey = null)
    {
        var tableDefiniton = InternalGetTable(context, tableId, roleId, tableName);
        var tableID = tableDefiniton.Id;

        Func<CellsModel, bool> searchFunc;

        if (rowNum is not null) searchFunc = (cell => cell.TableDefinitionsId == tableID && cell.RowNum == rowNum);
        else if (!string.IsNullOrEmpty(rowKey)) searchFunc = (cell => cell.TableDefinitionsId == tableID && !string.IsNullOrEmpty(cell.RowKey) && cell.RowKey.Equals(rowKey));
        else searchFunc = (cell => cell.TableDefinitionsId == tableID);

        var cells = context.Cells.Where(searchFunc);

        if (!string.IsNullOrEmpty(columnName) || columnId is not null)
        {
            int columnID;

            if (columnId is not null) columnID = (int)columnId!;
            else
            {
                var columnDefiniton = InternalGetColumn(context, columnName: columnName, tableID: tableID);
                columnID = columnDefiniton.Id;
            }

            cells = cells.Where(cell => cell.ColumnDefinitionsId == columnID);
        }

        return cells?.ToList() ?? new List<CellsModel>();
    }

    /// <summary>
    /// Basic gate keeping.  Needs at least 1 table identifier, 1 column identifier, and 1 row identifier or else will throw exceptions.
    /// Does -not- check if the given identifiers are valid, just that they're not null.
    /// </summary>
    /// <param name="tableId"></param>
    /// <param name="roleId"></param>
    /// <param name="tableName"></param>
    /// <param name="columnId"></param>
    /// <param name="columnName"></param>
    /// <param name="rowNum"></param>
    /// <param name="rowKey"></param>
    /// <exception cref="TableDoesNotExistException"></exception>
    /// <exception cref="ColumnDoesNotExistException"></exception>
    /// <exception cref="InvalidKeyException"></exception>
    private void CellGateKeep(
        int? tableId = null, ulong? roleId = null, string? tableName = null,
        int? columnId = null, string? columnName = null,
        int? rowNum = null, string? rowKey = null)
    {
        if (tableId is null && roleId is null && tableName is null && columnId is null) throw new TableDoesNotExistException("NULL table.");
        if (columnId is null && columnName is null) throw new ColumnDoesNotExistException("NULL column.");
        if (rowNum is null && string.IsNullOrEmpty(rowKey)) throw new InvalidKeyException("NULL rowNum and rowKey.");
    }
}
