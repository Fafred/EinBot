namespace EinBotDB.Models;

using System.ComponentModel.DataAnnotations;

public class CollectionTypesModel
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual ICollection<TableDefinitionsModel> TableDefinitions { get; set; }
}
