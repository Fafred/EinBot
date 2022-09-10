namespace EinBotDB.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class DataTypesModel
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<ColumnDefinitionsModel> ColumnDefinitions { get; set; }
}
