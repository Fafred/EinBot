namespace DataAccess.Data;

using DataAccess.DbAccess;
using DataAccess.Models;

public class CellsData : ICellsData
{
    private readonly IRelationalDataAccess _db;

    public CellsData(IRelationalDataAccess db)
    {
        _db = db;
    }

    public Task<IEnumerable<CellsModel>> GetCells() =>
        _db.LoadData<CellsModel, dynamic>(storedProcedure: "dbo.spCells_GetAll", new { });

    public async Task<CellsModel?> GetCell(int id)
    {
        var results = await _db.LoadData<CellsModel, dynamic>(
            storedProcedure: "dbo.spCells_Get",
            new { Id = id });

        return results.FirstOrDefault();
    }

    public Task InsertCell(CellsModel cellsModel) =>
        _db.SaveData(
            storedProcedure: "dbo.spCells_Insert",
            new
            {
                TableDefinitionsId = cellsModel.TableDefinitionsId,
                ColumnsDefinitionId = cellsModel.ColumnDefinitionsId,
                RowNum = cellsModel.RowNum,
                Data = cellsModel.Data,
                RowKey = cellsModel.Key
            });

    public Task UpdateCells(CellsModel cellsModel) =>
        _db.SaveData(storedProcedure: "dbo.spCells_Update", cellsModel);

    public Task DeleteCell(int id) =>
        _db.SaveData(storedProcedure: "dbo.spCells_Delete", new { Id = id });
}
