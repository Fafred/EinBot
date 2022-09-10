namespace EinBotDB.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InvalidKeyException : Exception
{
    public InvalidKeyException(string tableName, string columnName, string keyName) : base($"Table: {tableName}\tColumn: {columnName}\tKey: {keyName}") { }
}
