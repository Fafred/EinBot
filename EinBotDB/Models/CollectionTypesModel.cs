namespace EinBotDB.Models;

using System.ComponentModel.DataAnnotations;

public class CollectionTypesModel
{
    [Key]
    public int Id { get; init; }
    public string Name { get; set; }

    public virtual ICollection<TableDefinitionsModel> TableDefinitions { get; set; }
}
