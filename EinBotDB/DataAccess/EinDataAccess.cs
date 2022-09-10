namespace EinBotDB.DataAccess;

using EinBotDB.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EinDataAccess
{
    private EinDataContextFactory _factory;

    public EinDataAccess(EinDataContextFactory factory)
    {
        _factory = factory;
    }

    public EinTable GetEinTable(string tableName)
    {
        return new EinTable(tableName, _factory.CreateDbContext(new string[] { }));
    }
}
