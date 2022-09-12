namespace EinBotDB.DataAccess;

using EinBotDB.Context;
using EinBotDB.Models;
using System;

public partial class EinDataAccess
{
    /// <summary>
    /// Add a row to a table.
    /// </summary>
    /// <param name="tableId">The id of the table to add a row to.</param>
    /// <param name="key">The key of the role.</param>
    /// <param name="columnsDataDict">A dictionary with Key:Value of ColumnName:Data</param>
    /// <returns>The row number of the newly insterted row.</returns>
    /// <exception cref="InvalidDataException">If one of the given data types doesn't match the data type defined by the column.</exception>
    public int AddRow(int tableId, string key, Dictionary<string, string>? columnsDataDict)
    {
        using var context = _factory.CreateDbContext();

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

                if (!IsValidValue(columnDef.Id, data, context)) throw new InvalidDataException($"Column {columnDef.Name} expects {columnDef.DataTypes.Name}.  Received: {data}");
                
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

        foreach(var cell in cells)
        {
            if (columnsDataDict.ContainsKey(cell.ColumnDefinitions.Name))
            {
                var data = columnsDataDict[cell.ColumnDefinitions.Name];

                if (!IsValidValue(cell.ColumnDefinitionsId, data, context)) throw new InvalidDataException($"Column {cell.ColumnDefinitions.Name} expects {cell.ColumnDefinitions.DataTypes.Name}.  Received: {data}"); ;

                cell.Data = data;
            }
        }

        context.SaveChanges();

        return false;
    }


    private bool IsValidValue(int columnId, string data, EinDataContext context)
    {
        var column = context.ColumnDefinitions.FirstOrDefault(column => column.Id == columnId);

        if (column is null) throw new ColumnDoesNotExistException("Unknown", columnId);

        DataTypesEnum dataType = (DataTypesEnum)column.DataTypesId;

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
