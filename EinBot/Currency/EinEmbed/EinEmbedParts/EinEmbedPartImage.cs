namespace EinBot.Currency.EinEmbed;

using Discord;
using EinBotDB.DataAccess;
using EinBotDB.Models;

internal class EinEmbedPartImage : EinEmbedPartBase
{
    public EinEmbedPartImage(EinRow einRow, EinEmbedPartsModel partModel) : base(einRow, partModel) { }

    public override EmbedBuilder AddPartToEmbedBuilder(EmbedBuilder embedBuilder)
    {
        return embedBuilder.WithImageUrl(_data01);
    }
}
