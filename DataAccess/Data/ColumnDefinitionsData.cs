namespace DataAccess.Data;

using DataAccess.DbAccess;
using DataAccess.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ColumnDefinitionsData : IColumnDefinitionsData
{
    private readonly IRelationalDataAccess _db;

    public ColumnDefinitionsData(IRelationalDataAccess db)
    {
        _db = db;
    }


    public Task<IEnumerable<ColumnDefinitionsModel>> GeColumnDefinitions() =>
        _db.LoadData<ColumnDefinitionsModel, dynamic>(storedProcedure: "dbo.spColumnDefinitions_GetAll", new { });

    public async Task<ColumnDefinitionsModel?> GetColumnDefinition(int id)
    {
        var results = await _db.LoadData<ColumnDefinitionsModel, dynamic>(
            storedProcedure: "dbo.spColumnDefinitions_Get",
            new { Id = id });

        return results.FirstOrDefault();
    }

    public Task InsertColumnDefinition(ColumnDefinitionsModel columnDefinition) =>
        _db.SaveData(
            storedProcedure: "dbo.spColumnDefinitions_Insert",
            new
            {
                columnDefinition.TableDefinitionsId,
                columnDefinition.ColumnName,
                columnDefinition.DataTypesId,
            });

    public Task UpdateColumnDefinition(ColumnDefinitionsModel columnDefinition) =>
        _db.SaveData(storedProcedure: "dbo.spColumnDefinitions_Update", columnDefinition);

    public Task DeleteColumnDefinition(int id) =>
        _db.SaveData(storedProcedure: "dbo.spColumnDefinitions_Delete", new { Id = id });
}
