namespace EinBotDB.Exceptions;

using System;

public class ColumnDoesNotExistException : Exception
{
	public ColumnDoesNotExistException(string tableName, string columnName) : base($"Column Does Not Exist. Table: {tableName}\tColumn: {columnName}") { }
}
