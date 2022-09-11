namespace EinBotDB.DataAccess;

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
                if (IsValidValue(columnDef.Id, data)) cellModel.Data = data;
            }

            context.Cells.Add(cellModel);
        }

        context.SaveChanges();

        return curRow;
    }

    private bool IsValidValue(int columnId, string data)
    {
        throw new NotImplementedException();
    }
}
