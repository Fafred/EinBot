namespace EinBot.Currency.EinEmbed;

using Discord;

public interface IEinEmbedPart
{
    public EmbedBuilder AddPartToEmbedBuilder(EmbedBuilder embedBuilder);
}
