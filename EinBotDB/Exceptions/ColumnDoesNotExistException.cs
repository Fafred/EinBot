namespace EinBotDB;

using System;

public class ColumnDoesNotExistException : Exception
{
    public ColumnDoesNotExistException(string? message) : base(message) { }
    public ColumnDoesNotExistException(string tableName, string columnName) : base($"Column Does Not Exist. Table: {tableName}\tColumn name: {columnName}") { }
    public ColumnDoesNotExistException(string tableName, int columnId) : base($"Column Does Not Exist.  Table: {tableName}\tColumn id: {columnId}") { }
}
