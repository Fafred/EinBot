namespace EinBotDB.DataAccess;

using EinBotDB.Context;
using EinBotDB.Models;
using System;

public partial class EinDataAccess
{

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

                if (!IsValidValue(columnDef.Id, data, context)) throw new InvalidDataException();
                
                cellModel.Data = data;
            }

            context.Cells.Add(cellModel);
        }

        context.SaveChanges();

        return curRow;
    }

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

                if (!IsValidValue(cell.ColumnDefinitionsId, data, context)) throw new InvalidDataException();

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
