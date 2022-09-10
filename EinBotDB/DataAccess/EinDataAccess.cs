namespace EinBotDB.DataAccess;

using EinBotDB.Context;
using Microsoft.EntityFrameworkCore;

public partial class EinDataAccess
{
    private readonly IDbContextFactory<EinDataContext> _factory;

    public EinDataAccess(IDbContextFactory<EinDataContext> factory)
    {
        _factory = factory;
    }

    public EinTable GetEinTable(string tableName)
    {
        using var context = _factory.CreateDbContext();

        return new EinTable(tableName, context);
    }

    public EinTable GetEinTable(int tableId)
    {
        using var context = _factory.CreateDbContext();

        return new EinTable(tableId, context);
    }

    public EinTable GetEinTable(ulong roleId)
    {
        using var context = _factory.CreateDbContext();

        return new EinTable(roleId, context);
    }


}
