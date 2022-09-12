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
    /// <param name="tableName">Name of the table.</param>
    /// <returns>An EinTable of the given table.</returns>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given table name.</exception>
    public EinTable GetEinTable(string tableName)
    {
        using var context = _factory.CreateDbContext();

        return new EinTable(tableName, context);
    }

    /// <summary>
    /// Creates and returns an EinTable object from the given table name.
    /// </summary>
    /// <param name="tableId">The id of the table.</param>
    /// <returns>An EinTable of the given table.</returns>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given table id.</exception>
    public EinTable GetEinTable(int tableId)
    {
        using var context = _factory.CreateDbContext();

        return new EinTable(tableId, context);
    }

    /// <summary>
    /// Creates and returns an EinTable object from the given table name.
    /// </summary>
    /// <param name="roleId">Role id of the table.</param>
    /// <returns>An EinTable of the given table.</returns>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given role id.</exception>
    public EinTable GetEinTable(ulong roleId)
    {
        using var context = _factory.CreateDbContext();

        return new EinTable(roleId, context);
    }


}
