namespace EinBotDB.Models;

public class EinEmbedPartsModel
{
    public int Id { get; set; }

    public int TableDefinitionsId { get; set; }
    public virtual TableDefinitionsModel TableDefinitions { get; set; }

    public int EmbedPartTypesId { get; set; }
    public virtual EmbedPartTypesModel EmbedPartTypes { get; set; }

    public int Sequence { get; set; }

    public string? Data01 { get; set; }
    public string? Data02 { get; set; }
    public string? Data03 { get; set; }
}
