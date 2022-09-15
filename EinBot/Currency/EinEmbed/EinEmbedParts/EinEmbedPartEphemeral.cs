namespace EinBot.Currency.EinEmbed;

using Discord;
using EinBotDB.DataAccess;
using EinBotDB.Models;

internal class EinEmbedPartEphemeral : EinEmbedPartBase
{
    public EinEmbedPartEphemeral(EinRow einRow, EinEmbedPartsModel partModel) : base(einRow, partModel) { }

    public override EmbedBuilder AddPartToEmbedBuilder(EmbedBuilder embedBuilder)
    {
        return embedBuilder;
    }
}
