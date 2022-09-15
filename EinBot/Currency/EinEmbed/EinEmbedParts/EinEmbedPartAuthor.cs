namespace EinBot.Currency.EinEmbed;

using Discord;
using EinBotDB.DataAccess;
using EinBotDB.Models;

internal class EinEmbedPartAuthor : EinEmbedPartBase
{
    public EinEmbedPartAuthor(EinRow einRow, EinEmbedPartsModel partModel) : base(einRow, partModel) { }

    public override EmbedBuilder AddPartToEmbedBuilder(EmbedBuilder embedBuilder)
    {
        string? imgUrl = null;
        string? urlStr = null;

        if (!string.IsNullOrEmpty(_data02)) imgUrl = _data02;
        if (!string.IsNullOrEmpty(_data03)) urlStr = _data03;

        return embedBuilder.WithAuthor(_data01, iconUrl: imgUrl, url: urlStr);
    }
}
