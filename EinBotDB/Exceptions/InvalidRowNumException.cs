namespace EinBotDB;
using System;

public class InvalidRowNumException : Exception
{
    public InvalidRowNumException(string tableName, string columnName, int rowNum) : base($"Table: {tableName}\tColumn: {columnName}\tRowNum: {rowNum}") { }
    public InvalidRowNumException(int tableId, int rowNum) : base($"Invalid row on table id: {tableId}, RowNum {rowNum}") { }
}
