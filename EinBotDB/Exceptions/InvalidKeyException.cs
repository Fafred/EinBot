namespace EinBotDB;
using System;

public class InvalidKeyException : Exception
{
    public InvalidKeyException(string tableName, string columnName, string keyName) : base($"Table: {tableName}\tColumn: {columnName}\tKey: {keyName}") { }
    public InvalidKeyException(int tableId, string KeyName) : base($"Invalid key on table id: {tableId}, key {KeyName}") { }
}
