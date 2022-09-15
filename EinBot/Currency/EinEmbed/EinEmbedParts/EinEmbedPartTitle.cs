namespace EinBot.Currency.EinEmbed;

using Discord;
using EinBotDB.DataAccess;
using EinBotDB.Models;

internal class EinEmbedPartTitle : EinEmbedPartBase
{
    public EinEmbedPartTitle(EinRow einRow, EinEmbedPartsModel partModel) : base(einRow, partModel) { }

    public override EmbedBuilder AddPartToEmbedBuilder(EmbedBuilder embedBuilder)
    {
        if (string.IsNullOrEmpty(_data01)) return embedBuilder;

        return embedBuilder.WithTitle(_data01);
    }
}