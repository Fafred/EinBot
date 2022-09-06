namespace DataAccess.Models;

public class CellsModel
{
    public int Id { get; set; }
    public int TableDefinitionsId { get; set; }
    public int ColumnDefinitionsId { get; set; }
    public int RowNum { get; set; }
    public string Data { get; set; } = "";
    public string Key { get; set; } = "";
}
