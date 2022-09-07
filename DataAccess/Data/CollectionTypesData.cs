namespace DataAccess.Data;

using DataAccess.DbAccess;
using DataAccess.Models;

public class CollectionTypesData : ICollectionTypesData
{
    private readonly IRelationalDataAccess _db;

    public CollectionTypesData(IRelationalDataAccess db)
    {
        _db = db;
    }

    public Task<IEnumerable<CollectionTypesModel>> GetCollectionTypes() =>
        _db.LoadData<CollectionTypesModel, dynamic>(
            storedProcedure: "dbo.spCollectionTypes_GetAll",
            new { });

    public async Task<CollectionTypesModel?> GetCollectionType(int id)
    {
        var result = await _db.LoadData<CollectionTypesModel, dynamic>(
            storedProcedure: "dbo.spCollectionTypes_Get",
            new { Id = id });

        return result.FirstOrDefault();
    }
}
