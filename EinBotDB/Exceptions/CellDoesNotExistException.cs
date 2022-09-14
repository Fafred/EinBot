namespace EinBotDB;

using System;
using System.Runtime.Serialization;

[Serializable]
public class CellDoesNotExistException : Exception
{
    private int? tableId;
    private string columnName;
    private int? rowNum;
    private string? rowKey;

    public CellDoesNotExistException()
    {
    }

    public CellDoesNotExistException(string? message) : base(message)
    {
    }

    public CellDoesNotExistException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public CellDoesNotExistException(int? tableId, string columnName, int? rowNum) : base($"No cell exists for table id {tableId}, column name {columnName}, row num {rowNum}")
    {
        this.tableId = tableId;
        this.columnName = columnName;
        this.rowNum = rowNum;
    }

    public CellDoesNotExistException(int? tableId, string columnName, string? rowKey) : base($"No cell exists for table id {tableId}, column name {columnName}, row key {rowKey}")
    {
        this.tableId = tableId;
        this.columnName = columnName;
        this.rowKey = rowKey;
    }

    protected CellDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}