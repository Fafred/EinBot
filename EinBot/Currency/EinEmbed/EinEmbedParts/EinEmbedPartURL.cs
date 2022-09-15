namespace EinBot.Currency.EinEmbed;

using Discord;
using EinBotDB.DataAccess;
using EinBotDB.Models;

internal class EinEmbedPartURL : EinEmbedPartBase
{
    public EinEmbedPartURL(EinRow einRow, EinEmbedPartsModel partModel) : base(einRow, partModel) { }

    public override EmbedBuilder AddPartToEmbedBuilder(EmbedBuilder embedBuilder)
    {
        if (string.IsNullOrEmpty(_data01)) return embedBuilder;

        return embedBuilder.WithUrl(_data01);
    }
}
