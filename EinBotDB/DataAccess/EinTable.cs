namespace EinBotDB.DataAccess;

using EinBotDB.Context;
using EinBotDB.Exceptions;
using EinBotDB.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class EinTable
{
    private Dictionary<string, Type> _columnTypes = new Dictionary<string, Type>();

    private int _tableId;
    public int TableId { get { return _tableId; } }

    private string _tableName;
    public string Name { get { return _tableName; } }

    private int _collectionTypeId;
    public int CollectionTypeId { get { return _collectionTypeId; } }

    private string _collectionTypeName;
    public string CollectionTypeName { get { return _collectionTypeName; } }

    public ImmutableDictionary<string, DataTypesEnum> ColumnDataTypes;

    public ImmutableList<EinRow> Rows { get; private set; }

    internal EinTable(string tableName, EinDataContext context)
    {
        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(x =>
            x.Name == tableName);

        if (tableDefinition is null) throw new TableDoesNotExistException(tableName);
        LoadTable(tableDefinition.Id, context);
    }

    internal EinTable(ulong roleId, EinDataContext context)
    {
        TableDefinitionsModel? tableDefinition = context.TableDefinitions.FirstOrDefault(x =>
            x.RoleId == roleId);

        if (tableDefinition is null) throw new TableDoesNotExistException(roleId);
        LoadTable(tableDefinition.Id, context);
    }

    internal EinTable(int tableId, EinDataContext context)
    {
        LoadTable(tableId, context);
    }

    private void LoadTable(int tableId, EinDataContext context)
    {
        // First grab the table definition.  This will tell us the table's name and collectin type.
        TableDefinitionsModel? table;

        table = context.TableDefinitions.FirstOrDefault(x =>
            x.Id == tableId);

        if (table is null) throw new TableDoesNotExistException(tableId);

        _tableId = tableId;
        _tableName = table.Name;
        _collectionTypeId = table.CollectionTypeId;

        CollectionTypesModel? collectionType;

        collectionType = context.CollectionTypes.FirstOrDefault(x =>
            x.Id == _collectionTypeId);

        // Verify the collection type actually exists.
        if (collectionType is null) throw new InvalidKeyException(_tableName, "CollectionTypeId", _collectionTypeId.ToString());

        _collectionTypeName = collectionType.Name;

        // Now we grab the column definitions and set up the dictionary.
        var columnDefinitions = context.ColumnDefinitions.Where(x =>
            x.TableDefinitionsId == _tableId);

        Dictionary<string, DataTypesEnum> dict = new Dictionary<string, DataTypesEnum>();
        foreach(var columnDefinition in columnDefinitions)
        {
            dict[columnDefinition.Name] = (DataTypesEnum)columnDefinition.DataTypesId;
        }
        ColumnDataTypes = dict.ToImmutableDictionary();

        // Now we grab the "rows".
        List<EinRow> einRowsList = new List<EinRow>();

        var rows = context.Cells.Where(x =>
            x.TableDefinitionsId == _tableId);

        var rowNums =
            (from row in rows
             select row.RowNum).Distinct();

        foreach(var rowNum in rowNums)
        {
            var cells = rows.Where(x =>
                x.RowNum == rowNum).ToList();

            einRowsList.Add(new EinRow(this, ColumnDataTypes, cells));
        }

        Rows = einRowsList.ToImmutableList();
    }
}
