namespace EinBotDB.Models;

using System.ComponentModel.DataAnnotations;

public class CellsModel
{
    [Key]
    public int Id { get; init; }

    public int TableDefinitionsId { get; set; }
    public virtual TableDefinitionsModel TableDefinitions { get; set; }

    public int ColumnDefinitionsId { get; set; }
    public virtual ColumnDefinitionsModel ColumnDefinitions { get; set; }

    public int RowNum { get; set; }

    public string? Data { get; set; }

    public string? RowKey { get; set; }
}
