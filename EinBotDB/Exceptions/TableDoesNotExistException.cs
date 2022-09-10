namespace EinBotDB.Exceptions;

using System;

public class TableDoesNotExistException : Exception
{
	private string _tableName;

	public TableDoesNotExistException() { }

	public TableDoesNotExistException(string tableName) : base(tableName)
	{
		_tableName = tableName;	
	}

	public TableDoesNotExistException(string tableName, Exception innerException) : base(tableName, innerException)
	{
		_tableName = tableName;
	}

}
