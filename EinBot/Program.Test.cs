namespace EinBot;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using EinBotDB.Context;
using EinBotDB.DataAccess;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public partial class Program
{
    public async Task Testing()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
                services
                    .AddDbContextFactory<EinDataContext>(options => options.UseSqlite(configuration.GetConnectionString("EinBotDb")))
                    .AddSingleton<EinDataAccess>())
            .Build();


        ServiceProvider = host.Services;

        var dal = ServiceProvider.GetRequiredService<EinDataAccess>();


        Dictionary<string, string> columnsDataDict = new Dictionary<string, string>()
        {
            {"WholeNumber", "100"},
            {"TextField", "This is a new text field." },
            {"ListOfStrings", "One element in a list." }
        };

        dal.CreateTable("GoldTable", CollectionTypesEnum.PerKey);
        dal.CreateColumn(2, "Gold", DataTypesEnum.Int);

        dal.AddRow(2, "SoAndSo", new Dictionary<string, string>() { { "Gold", "41" } });

        var table = dal.GetEinTable(2);

        foreach (var row in table.Rows)
        {
            Console.WriteLine(row);
        }


        Console.WriteLine("---");


        await Task.Delay(-1);
    }
}
