namespace EinBotDB.DataAccess;

using EinBotDB.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

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
                /*case DataTypesEnum.ListInt:
                case DataTypesEnum.ListDecimal:
                case DataTypesEnum.ListText:
                case DataTypesEnum.ListUserId:
                case DataTypesEnum.ListGuildId:
                case DataTypesEnum.ListChannelId:
                case DataTypesEnum.ListRoleId:
                    if (!ListColumns.ContainsKey(columnName))
                    {
                        ListColumns.Add(columnName, new List<string?>());
                    }
                    ListColumns[columnName].Add(cell.Data);
                    break;*/
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

        foreach (string key in ColumnDataTypes.Keys)
        {
            stringBuilder.Append($"[{key}: ");

            if (Columns.ContainsKey(key))
            {
                stringBuilder.Append(Columns[key] ?? "NULL");
                stringBuilder.Append("] ");
            }
            else if (ListColumns.ContainsKey(key))
            {
                stringBuilder.Append('(');

                foreach (var str in ListColumns[key])
                {
                    stringBuilder.Append(str ?? "NULL");
                    stringBuilder.Append(", ");
                }
                stringBuilder.Append(")] ");
            }
            else
            {
                stringBuilder.Append("NULL] ");
            }
        }

        _string = stringBuilder.ToString();
        return _string;
    }

    internal string ToCSVString(List<string> columnsList)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append($"{Id},{Key},");

        for (int i = 0; i < columnsList.Count; ++i)
        {
            string columnName = columnsList[i];

            if (!Columns.ContainsKey(columnName))
            {
                sb.Append(',');
                continue;
            }

            if (IsColumnList(columnName)) throw new NotImplementedException("EinRow.ToCSVString: List columns.");

            sb.Append(Columns[columnName]);

            if (i < columnsList.Count - 1) sb.Append(',');
        }

        return sb.ToString();
    }
}

