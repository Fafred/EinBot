namespace EinBotDB.DataAccess;

using EinBotDB;
using EinBotDB.Models;
using System.Text;

public partial class EinDataAccess
{
    /// <summary>
    /// Adds a part to the embed display.
    /// </summary>
    /// <param name="embedPart">The <see cref="EmbedPartsEnum">Embed Part ID</see> the part is.</param>
    /// <param name="Data01">The data in the first slot.</param>
    /// <param name="Data02">Null or the data in the first slot.</param>
    /// <param name="Data03">Null or the data in the first slot.</param>
    /// <param name="tableId">Null or the table id the display is attached to.  If null, then either role id or tablename cannot be.</param>
    /// <param name="roleId">Null or the role id the display is attached to.  If null, then either table id or tablename cannot be.</param>
    /// <param name="tableName">Null or the name the display is attached to.  If null, then either role id or table id cannot be.</param>
    /// <param name="sequence">The order # of the part.  If null will place it at the end.</param>
    /// <exception cref="TableDoesNotExistException">If no such table exists.</exception>"
    /// <returns>The id of the new <see cref="EinEmbedPartsModel"/></returns>
    public int AddEmbedPart(EmbedPartsEnum embedPart,
        string Data01, string? Data02 = null, string? Data03 = null,
        int? tableId = null, ulong? roleId = null, string? tableName = null,
        int? sequence = null)
    {
        using var context = _factory.CreateDbContext();

        var tableDefinition = InternalGetTable(context, tableId, roleId, tableName);

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
            Data01 = ReplaceNewLinesAndTabs(Data01),
            Data02 = ReplaceNewLinesAndTabs(Data02 ?? ""),
            Data03 = ReplaceNewLinesAndTabs(Data03 ?? ""),
            Sequence = seq,
        });

        context.SaveChanges();
        return addedEmbed.Entity.Id;
    }


    /// <summary>
    /// Retrieves the sequence # of the given part.
    /// </summary>
    /// <param name="embedPartId">The id of the part to get the sequence of.</param>
    /// <returns>The sequence # of the part.</returns>
    /// <exception cref="EinEmbedPartDoesNotExistException">If there's no part with the given id./exception>
    public int GetEmbedPartSequence(int embedPartId)
    {
        using var context = _factory.CreateDbContext();

        var embedPart = context.EinEmbedParts.FirstOrDefault(part => part.Id == embedPartId);

        if (embedPart is null) throw new EinEmbedPartDoesNotExistException(embedPartId);

        return embedPart.Sequence;
    }

    /// <summary>
    /// Retrieves the embed part with the given Id which is attached to the given table.
    /// </summary>
    /// <param name="embedPartId">The id of the part.</param>
    /// <param name="tableId">Null or the table id the embed is attached to.  If null, then either role id or tablename cannot be.</param>
    /// <param name="roleId">Null or the role id the embed is attached to.  If null, then either table id or tablename cannot be.</param>
    /// <param name="tableName">Null or the name the embed is attached to.  If null, then either role id or table id cannot be.</param>
    /// <returns>The <see cref="EinEmbedPartsModel"/> or null if none exists which matches the criteria.</returns>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    public EinEmbedPartsModel? GetEmbedPart(int embedPartId, int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        var embedPartList = GetEmbedPartsHelper(embedPartId: embedPartId, tableId: tableId, roleId: roleId, tableName: tableName);

        if (embedPartList is not null) return embedPartList.First();

        return null;
    }

    /// <summary>
    /// Retrieves a list of the embed parts which are attached to the given table.
    /// </summary>
    /// <param name="embedPartId">The id of the part.</param>
    /// <param name="tableId">Null or the table id the embeds are attached to.  If null, then either role id or tablename cannot be.</param>
    /// <param name="roleId">Null or the role id the embeds are attached to.  If null, then either table id or tablename cannot be.</param>
    /// <param name="tableName">Null or the name the embeds are attached to.  If null, then either role id or table id cannot be.</param>
    /// <returns>A List of <see cref="EinEmbedPartsModel"/> or null if none exist.</returns>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    public List<EinEmbedPartsModel>? GetEmbedParts(int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        var partsList = GetEmbedPartsHelper(tableId: tableId, roleId: roleId, tableName: tableName);

        if (partsList is not null)
        {
            partsList.Sort((a, b) => a.Sequence.CompareTo(b.Sequence));
        }

        return partsList;
    }

    /// <summary>
    /// Removes all the embed parts attached to the given table.
    /// </summary>
    /// <param name="tableId">Null or the table id the embeds are attached to.  If null, then either role id or tablename cannot be.</param>
    /// <param name="roleId">Null or the role id the embeds are attached to.  If null, then either table id or tablename cannot be.</param>
    /// <param name="tableName">Null or the name the embeds are attached to.  If null, then either role id or table id cannot be.</param>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    public void RemoveAllEmbedParts(int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        using var context = _factory.CreateDbContext();

        var tableDefinition = InternalGetTable(context, tableId: tableId, roleId: roleId, tableName: tableName);

        var partsList = context.EinEmbedParts.Where(part => part.TableDefinitionsId == tableDefinition.Id);
        context.EinEmbedParts.RemoveRange(partsList);

        context.SaveChanges();
    }

    /// <summary>
    /// Removes the embed part with the given id.
    /// </summary>
    /// <param name="embedPartId">The id of the part to remove.</param>
    /// <exception cref="EinEmbedPartDoesNotExistException">If no embed exists with the given id.</exception>
    public void RemoveEmbedPart(int embedPartId)
    {
        using var context = _factory.CreateDbContext();

        var embedPart = context.EinEmbedParts.FirstOrDefault(part => part.Id == embedPartId);

        if (embedPart is null) throw new EinEmbedPartDoesNotExistException(embedPartId);

        context.EinEmbedParts.Remove(embedPart);

        context.SaveChanges();
    }

    /// <summary>
    /// Sets the data in the given slot for the given part.
    /// </summary>
    /// <param name="embedPartId">The id to set the data for.</param>
    /// <param name="dataSeq">1, 2, or 3.  If > 3, then 3.  If < 1, then 1.</param>
    /// <param name="data">The new data to set.</param>
    /// <exception cref="EinEmbedPartDoesNotExistException">If there's no embed part with the given id.</exception>
    public void SetEmbedPartData(int embedPartId, int dataSeq, string data)
    {
        using var context = _factory.CreateDbContext();

        var embedPart = context.EinEmbedParts.FirstOrDefault(part => part.Id == embedPartId);

        if (embedPart is null) throw new EinEmbedPartDoesNotExistException(embedPartId);

        switch (dataSeq)
        {
            case 1:
                embedPart.Data01 = ReplaceNewLinesAndTabs(data);
                break;
            case 2:
                embedPart.Data02 = ReplaceNewLinesAndTabs(data);
                break;
            case 3:
                embedPart.Data03 = ReplaceNewLinesAndTabs(data);
                break;
        }

        context.SaveChanges();
    }

    /// <summary>
    /// Changes the sequence (order) # the part is applied to the display.
    /// </summary>
    /// <param name="embedPartId">The id of the part to change the sequence of.</param>
    /// <param name="newSequenceNum">The new sequence number.</param>
    /// <exception cref="EinEmbedPartDoesNotExistException">If the part does not exist.</exception>
    public void SetEmbedPartSequence(int embedPartId, int newSequenceNum)
    {
        using var context = _factory.CreateDbContext();

        var embedPart = context.EinEmbedParts.FirstOrDefault(part => part.Id == embedPartId);

        if (embedPart is null) throw new EinEmbedPartDoesNotExistException(embedPartId);

        embedPart.Sequence = newSequenceNum;

        context.SaveChanges();
    }

    /******
     * 
     * PRIVATE
     * 
     ******/

    private List<EinEmbedPartsModel>? GetEmbedPartsHelper(int? embedPartId = null, int? tableId = null, ulong? roleId = null, string? tableName = null)
    {
        using var context = _factory.CreateDbContext();

        var tableDefinition = InternalGetTable(context, tableId: tableId, roleId: roleId, tableName: tableName);

        tableId = tableDefinition.Id;

        List<EinEmbedPartsModel> partsList = new List<EinEmbedPartsModel>();

        if (embedPartId is null)
        {
            partsList = context.EinEmbedParts.Where(part => part.TableDefinitionsId == tableId).ToList();
        }
        else
        {
            var embedPart = context.EinEmbedParts.FirstOrDefault(part => part.TableDefinitionsId == tableId && part.Id == (int)embedPartId);

            if (embedPart is not null) partsList.Add(embedPart);
            else throw new EinEmbedPartDoesNotExistException((int)embedPartId);
        }

        if (partsList.Count < 1) return null;

        return partsList;
    }

    private string ReplaceNewLinesAndTabs(string data)
    {
        StringBuilder sb = new(data);

        sb.Replace("\\n", "\n");
        sb.Replace("\\t", "\t");
        return sb.ToString();
    }
}