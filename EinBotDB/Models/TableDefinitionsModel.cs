namespace EinBotDB.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

[Index(nameof(TableDefinitionsModel.Name), IsUnique = true)]
[Index(nameof(TableDefinitionsModel.RoleId), IsUnique = true)]
public class TableDefinitionsModel
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; }

    public ulong RoleId { get; set; }

    public int CollectionTypeId { get; set; }
    public virtual CollectionTypesModel CollectionType { get; set; }

    public virtual ICollection<CellsModel> Cells { get; set; }

    public virtual ICollection<ColumnDefinitionsModel> ColumnDefinitions { get; set; }

    public virtual ICollection<EinEmbedPartsModel> EinEmbedParts { get; set; }
}
