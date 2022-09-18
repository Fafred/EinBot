namespace EinBotDB.DataAccess;

using EinBotDB.Context;
using Microsoft.EntityFrameworkCore;

public partial class EinDataAccess : IEinDataAccess
{
    private readonly IDbContextFactory<EinDataContext> _factory;

    public EinDataAccess(IDbContextFactory<EinDataContext> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Creates and returns an EinTable object from the given table name.
    /// </summary>
    /// <param name="tableId">Id of the table.</param>
    /// <param name="roleId">Role id of the table</param>
    /// <param name="tableName">Name of the table</param>
    /// <returns>An EinTable of the given table.</returns>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given table name.</exception>
    public EinTable GetEinTable(int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        if (tableId is null && roleId is null && string.IsNullOrEmpty(tableName)) throw new TableDoesNotExistException("Null table.");
        using var context = _factory.CreateDbContext();

        if (tableId is not null) return new EinTable((int)tableId, context);
        else if (roleId is not null) return new EinTable((ulong)roleId, context);
        else return new EinTable(tableName!, context);
    }
}
