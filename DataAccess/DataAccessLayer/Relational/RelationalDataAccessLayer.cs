namespace DataAccess.DataAccessLayer.Relational;

using DataAccess.Data;
using DataAccess.DbAccess;
using DataAccess.Models;

public class RelationalDataAccessLayer
{
    private readonly IRelationalDataAccess _db;

    public RelationalDataAccessLayer(IRelationalDataAccess relationalDataAccess)
    {
        _db = relationalDataAccess;
    }

    public async Task<TableDefinitionsModel?> CreateTableDefinition(
        string tableName,
        CollectionTypeEnum collectionType,
        string? roleID)
    {
        TableDefinitionsData tdb = new TableDefinitionsData(_db);

        TableDefinitionsModel tableDefinition = new TableDefinitionsModel()
        {
            TableName = tableName,
            CollectionTypesId = (int)collectionType,
            RoleId = roleID
        };

        await tdb.InsertTableDefinition(tableDefinition);

        return tdb.GetTableDefinitionByName(tableName).Result;
    }
}
