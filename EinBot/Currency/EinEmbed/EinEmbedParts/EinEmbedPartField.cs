namespace EinBot.Currency.EinEmbed;

using Discord;
using EinBotDB.DataAccess;
using EinBotDB.Models;

internal class EinEmbedPartField : EinEmbedPartBase
{
    public EinEmbedPartField(EinRow einRow, EinEmbedPartsModel partModel) : base(einRow, partModel) { }

    public override EmbedBuilder AddPartToEmbedBuilder(EmbedBuilder embedBuilder)
    {
        bool isInfield;

        bool.TryParse(_data03, out isInfield);

        string title = (string.IsNullOrEmpty(_data01) ? "_ _" : _data01);
        string desc = (string.IsNullOrEmpty(_data02) ? "_ _" : _data02);

        return embedBuilder.AddField(title, desc, isInfield);
    }
}
