namespace EinBotDB.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class ColumnDefinitionsModel
{
    [Key]
    public int Id { get; set; }

    public int TableDefinitionsId { get; set; }
    public virtual TableDefinitionsModel TableDefinitions { get; set; }

    public int DataTypesId { get; set; }
    public virtual DataTypesModel DataTypes { get; set; }

    public string Name { get; set; }

    public virtual ICollection<CellsModel> Cells { get; set; }
}
