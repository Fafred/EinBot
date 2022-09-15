namespace EinBot.Currency.EinEmbed;

using Discord;
using EinBotDB.DataAccess;
using EinBotDB.Models;

internal class EinEmbedPartColor : EinEmbedPartBase
{
    public EinEmbedPartColor(EinRow einRow, EinEmbedPartsModel partModel) : base(einRow, partModel) { }

    public override EmbedBuilder AddPartToEmbedBuilder(EmbedBuilder embedBuilder)
    {
        if (int.TryParse(_data01, out int r)) r = Math.Max(Math.Min(r, 255), 0);
        if (int.TryParse(_data02, out int g)) r = Math.Max(Math.Min(g, 255), 0);
        if (int.TryParse(_data03, out int b)) r = Math.Max(Math.Min(b, 255), 0);

        return embedBuilder.WithColor(new Color(r, g, b));
    }
}
