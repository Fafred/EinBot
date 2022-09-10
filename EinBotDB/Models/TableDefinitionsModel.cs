namespace EinBotDB.Models;

using System.ComponentModel.DataAnnotations;

public class TableDefinitionsModel
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public ulong? RoleId { get; set; }

    public int CollectionTypeId { get; set; }
    public CollectionTypesModel CollectionType { get; set; }

    public virtual ICollection<CellsModel> Cells { get; set; }

    public virtual ICollection<ColumnDefinitionsModel> ColumnDefinitions { get; set; }
}
