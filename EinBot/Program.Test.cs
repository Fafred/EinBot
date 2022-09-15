namespace EinBot;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using EinBotDB.Context;
using EinBotDB.DataAccess;
using EinBotDB.Models;
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

        ulong testRoleId = 1019855692577898526;
        var tableDefinition = dal.CreateTable("EmbedTest", CollectionTypesEnum.PerRole, testRoleId);
        //var tableDefinition = dal.GetTable(testRoleId);
        var tableId = tableDefinition.Id;

        var titleId = dal.AddEmbedPart(EinBotDB.EmbedPartsEnum.Title, "Test Title", tableId: tableId);
        var descId = dal.AddEmbedPart(EinBotDB.EmbedPartsEnum.Description, "Test description", tableId: tableId);
        var field1Id = dal.AddEmbedPart(EinBotDB.EmbedPartsEnum.Field, "Field 1", Data03: "true", tableId: tableId);
        var field2Id = dal.AddEmbedPart(EinBotDB.EmbedPartsEnum.Field, "Field 2", Data03: "true", tableId: tableId);
        var field3Id = dal.AddEmbedPart(EinBotDB.EmbedPartsEnum.Field, "Field 3", Data03: "false", tableId: tableId);

        var idList = new List<int> { titleId, descId, field1Id, field2Id, field3Id };

        var field1Seq = dal.GetEmbedPartSequence(field1Id);
        dal.SetEmbedPartSequence(field3Id, field1Seq);
        dal.SetEmbedPartSequence(field1Id, field1Seq + 2);

        foreach(var embedPart in dal.GetEmbedParts(tableId: tableId) ?? new List<EinEmbedPartsModel>())
        {
            Console.WriteLine($"id: {embedPart.Id}\t seq: {embedPart.Sequence}\t data: {embedPart.Data01}");
        }

        dal.DeleteTable(tableId);

        await Task.Delay(-1);
    }
}
