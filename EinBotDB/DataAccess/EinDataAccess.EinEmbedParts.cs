namespace EinBotDB.DataAccess;

using EinBotDB.Context;
using EinBotDB;
using EinBotDB.Models;

public partial class EinDataAccess
{
    public int AddEmbedPart(EmbedPartsEnum embedPart, 
        string Data01, string? Data02 = null, string? Data03 = null,
        int? tableId = null, ulong? roleId = null, string? tableName = null,
        int? sequence = null)
    {
        using var context = _factory.CreateDbContext();

        var tableDefinition = GetTableHelper(context, tableId, roleId, tableName);

        tableId = tableDefinition.Id;

        // Use the given sequence, or if it's null then set it to the max sequence + 1.
        if (sequence is null)
        {
            sequence = 1;
            var embedParts = context.EinEmbedParts.Where(part => part.TableDefinitionsId == tableId);

            if (embedParts is not null && embedParts!.Count() > 0)
            {
                sequence = embedParts!.Max(ep => ep.Sequence) + 1;
            }
        }

        int seq = (int)sequence;

        var addedEmbed = context.EinEmbedParts.Add(new EinEmbedPartsModel()
            {
                TableDefinitionsId = (int)tableId,
                EmbedPartTypesId = (int)embedPart,
                Data01 = Data01,
                Data02 = Data02,
                Data03 = Data03,
                Sequence = seq,
            });

        context.SaveChanges();
        return addedEmbed.Entity.Id;
    }

    public void SetEmbedPartData(int embedPartId, int dataSeq, string data)
    {
        using var context = _factory.CreateDbContext();

        var embedPart = context.EinEmbedParts.FirstOrDefault(part => part.Id == embedPartId);

        if (embedPart is null) throw new EinEmbedPartDoesNotExistException(embedPartId);

        switch (dataSeq)
        {
            case 1:
                embedPart.Data01 = data;
                break;
            case 2:
                embedPart.Data02 = data;
                break;
            case 3:
                embedPart.Data03 = data;
                break;
        }

        context.SaveChanges();
    }

    public void SetEmbedPartSequence(int embedPartId, int newSequenceNum)
    {
        using var context = _factory.CreateDbContext();

        var embedPart = context.EinEmbedParts.FirstOrDefault(part => part.Id == embedPartId);

        if (embedPart is null) throw new EinEmbedPartDoesNotExistException(embedPartId);

        embedPart.Sequence = newSequenceNum;

        context.SaveChanges();
    }

    public int GetEmbedPartSequence(int embedPartId)
    {
        using var context = _factory.CreateDbContext();

        var embedPart = context.EinEmbedParts.FirstOrDefault(part => part.Id == embedPartId);

        if (embedPart is null) throw new EinEmbedPartDoesNotExistException(embedPartId);

        return embedPart.Sequence;
    }

    public EinEmbedPartsModel? GetEmbedPart(int embedPartId, int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        var embedPartList = GetEmbedPartsHelper(embedPartId: embedPartId, tableId: tableId, roleId: roleId, tableName: tableName);

        if (embedPartList is not null) return embedPartList.First();

        return null;
    }

    public List<EinEmbedPartsModel>? GetEmbedParts(int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        var partsList = GetEmbedPartsHelper(tableId: tableId, roleId: roleId, tableName: tableName);

        if (partsList is not null)
        {
            partsList.Sort((a, b) => a.Sequence.CompareTo(b.Sequence));
        }

        return partsList;
    }

    public void RemoveEmbedPart(int embedPartId)
    {
        using var context = _factory.CreateDbContext();

        var embedPart = context.EinEmbedParts.FirstOrDefault(part => part.Id == embedPartId);

        if (embedPart is null) throw new EinEmbedPartDoesNotExistException(embedPartId);

        context.EinEmbedParts.Remove(embedPart);

        context.SaveChanges();
    }

    private List<EinEmbedPartsModel>? GetEmbedPartsHelper(int? embedPartId = null, int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        using var context = _factory.CreateDbContext();

        var tableDefinition = GetTableHelper(context, tableId: tableId, roleId: roleId, tableName: tableName);

        tableId = tableDefinition.Id;

        List<EinEmbedPartsModel> partsList = new List<EinEmbedPartsModel>();

        if (embedPartId is null)
        {
            partsList = context.EinEmbedParts.Where(part => part.TableDefinitionsId == tableId).ToList();
        } else
        {
            var embedPart = context.EinEmbedParts.FirstOrDefault(part => part.TableDefinitionsId == tableId && part.Id == (int)embedPartId);

            if (embedPart is not null) partsList.Add(embedPart);
            else throw new EinEmbedPartDoesNotExistException((int)embedPartId);
        }

        if (partsList.Count < 1) return null;

        return partsList;
    }

    private TableDefinitionsModel GetTableHelper(EinDataContext context, int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        // Make sure the table exists.
        if (tableId is null && roleId is null && tableName is null) throw new TableDoesNotExistException("Null");

        TableDefinitionsModel? tableDefinition;
        Func<TableDefinitionsModel, bool> tableSearchFunc;

        if (tableId is not null) tableSearchFunc = (table => table.Id == tableId);
        else if (roleId is not null) tableSearchFunc = (table => table.RoleId == roleId);
        else tableSearchFunc = (table => table.Name.Equals(tableName));

        tableDefinition = context.TableDefinitions.FirstOrDefault(tableSearchFunc);

        if (tableDefinition is null) throw new TableDoesNotExistException(tableId: tableId, roleId: roleId, tableName: tableName);

        return tableDefinition;
    }
}