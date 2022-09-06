namespace DataAccess.Data;

using DataAccess.DbAccess;

public class CellsData
{
    private readonly ISqlDataAccess _db;

    public CellsData(ISqlDataAccess db)
    {
        _db = db;
    }


}
