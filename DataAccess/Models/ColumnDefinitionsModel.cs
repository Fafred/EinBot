namespace DataAccess.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ColumnDefinitionsModel
{
    public int Id { get; set; }
    public int TableDefinitionsId { get; set; }
    public int DataTypesId { get; set; }
    public string ColumnName { get; set; } = "";
}
