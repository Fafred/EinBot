namespace EinBotDB;

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ColumnAlreadyExistsException : Exception
{
    public ColumnAlreadyExistsException(int tableId, string columnName) : base($"Column already exists.  Table id: {tableId}\t Column name: {columnName}") { }
    public ColumnAlreadyExistsException(string tableName, string columnName) : base($"Column already exists.  Table id: {tableName}\t Column name: {columnName}") { }
}
