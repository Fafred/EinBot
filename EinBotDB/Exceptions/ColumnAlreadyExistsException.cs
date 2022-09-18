namespace EinBotDB;

using System;

public class ColumnAlreadyExistsException : Exception
{
    public ColumnAlreadyExistsException(int tableId, string columnName) : base($"Column already exists.  Table id: {tableId}\t Column name: {columnName}") { }
    public ColumnAlreadyExistsException(string tableName, string columnName) : base($"Column already exists.  Table id: {tableName}\t Column name: {columnName}") { }
}
