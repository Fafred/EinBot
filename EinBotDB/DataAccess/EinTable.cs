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
using System.Text;
using System.Threading.Tasks;

public class EinTable
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

    public ImmutableList<EinRow> Rows { get; }

    internal EinTable(string tableName, EinDataContext context)
    {
        // First grab the table definition.  This will tell us the table's name and collectin type.
        TableDefinitionsModel? table;

        table = context.TableDefinitions.FirstOrDefault(x =>
            x.Name == tableName);

        if (table is null) throw new TableDoesNotExistException(tableName);

        _tableId = table.Id;
        _tableName = tableName;
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

    public class EinRow
    {
        private string _string = "";

        public EinTable Table { get; }
        public ImmutableDictionary<string, DataTypesEnum> ColumnDataTypes { get; }

        public int Id { get; set; }
        public string? Key { get; set; }

        public Dictionary<string, string?> Columns = new Dictionary<string, string?>();
        public Dictionary<string, List<string?>> ListColumns = new Dictionary<string, List<string?>>();

        internal EinRow(EinTable table, ImmutableDictionary<string, DataTypesEnum> columnDataTypes, ICollection<CellsModel> cells)
        {
            Table = table;
            ColumnDataTypes = columnDataTypes;

            var firstCell = cells.First();

            if (firstCell is null) return;

            Id = firstCell.RowNum;
            Key = firstCell.RowKey;

            foreach (CellsModel cell in cells)
            {
                string columnName = cell.ColumnDefinitions.Name;

                switch (ColumnDataTypes[columnName])
                {
                    case DataTypesEnum.ListInt:
                    case DataTypesEnum.ListDecimal:
                    case DataTypesEnum.ListText:
                    case DataTypesEnum.ListUserId:
                    case DataTypesEnum.ListGuildId:
                    case DataTypesEnum.ListChannelId:
                        if (!ListColumns.ContainsKey(columnName))
                        {
                            ListColumns.Add(columnName, new List<string?>());
                        }
                        ListColumns[columnName].Add(cell.Data);
                        break;
                    default:
                        Columns[columnName] = cell.Data;
                        break;
                }

            }
        }

        public bool IsColumnList(string columnName)
        {
            return ListColumns.ContainsKey(columnName);
        }

        public (DataTypesEnum DataType, bool IsColumnList, dynamic? Data) this[string columnName]
        {
            get
            {
                if (!ColumnDataTypes.ContainsKey(columnName)) throw new ColumnDoesNotExistException(Table.Name, columnName);

                if (Columns.ContainsKey(columnName)) return (ColumnDataTypes[columnName], false, Columns[columnName]);

                return (ColumnDataTypes[columnName], true, ListColumns[columnName]);
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(_string)) return _string;

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"[{Id}] [Key: {Key ?? "NULL"}] ");

            foreach(string key in ColumnDataTypes.Keys)
            {
                stringBuilder.Append($"[{key}: ");

                if (Columns.ContainsKey(key))
                {
                    stringBuilder.Append(Columns[key] ?? "NULL");
                    stringBuilder.Append("] ");
                } else if (ListColumns.ContainsKey(key))
                {
                    stringBuilder.Append('(');

                    foreach(var str in ListColumns[key])
                    {
                        stringBuilder.Append(str ?? "NULL");
                        stringBuilder.Append(", ");
                    }
                    stringBuilder.Append(")] ");
                } else
                {
                    stringBuilder.Append("NULL] ");
                }
            }

            _string = stringBuilder.ToString();
            return _string;
        }
    }

    private void LoadTable(string tableName, EinDataContext context)
    {

    }
}
