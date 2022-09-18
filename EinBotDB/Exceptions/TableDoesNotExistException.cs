namespace EinBotDB;

using System;

public class TableDoesNotExistException : Exception
{
    public TableDoesNotExistException() { }

    public TableDoesNotExistException(int? tableId = null, ulong? roleId = null, string? tableName = null) : base($"table {tableId?.ToString() ?? ""}{roleId?.ToString() ?? ""}{tableName ?? ""} does not exist.") { }

    public TableDoesNotExistException(int tableId) : base(tableId.ToString()) { }
    public TableDoesNotExistException(int tableId, Exception innerException) : base($"table id: {tableId}", innerException) { }

    public TableDoesNotExistException(string tableName) : base(tableName) { }
    public TableDoesNotExistException(string tableName, Exception innerException) : base($"table name: {tableName}", innerException) { }

    public TableDoesNotExistException(ulong roleId) : base(roleId.ToString()) { }
    public TableDoesNotExistException(ulong roleId, Exception innerException) : base($"role id: {roleId}", innerException) { }
}
