namespace EinBotDB.Models;

public class EmbedPartTypesModel
{
    public int Id { get; init; }

    public string Name { get; set; }

    public virtual ICollection<EinEmbedPartsModel> EmbedParts { get; set; }
}
