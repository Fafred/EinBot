namespace DataAccess.Data;

using DataAccess.DbAccess;
using DataAccess.Models;

public class TypesData : ITypesData
{
    private readonly ISqlDataAccess _db;

    public TypesData(ISqlDataAccess db)
    {
        _db = db;
    }

    public Task<IEnumerable<DataTypesModel>> GetDataTypes() =>
        _db.LoadData<DataTypesModel, dynamic>(storedProcedure: "dbo.spDataTypes_GetAll", new { });

    public async Task<DataTypesModel?> GetDataType(int id)
    {
        var results = await _db.LoadData<DataTypesModel, dynamic>(
            storedProcedure: "dbo.spDataTypes_Get",
            new { Id = id });

        return results.FirstOrDefault();
    }
}
