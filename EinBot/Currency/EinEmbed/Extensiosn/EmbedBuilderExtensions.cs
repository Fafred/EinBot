namespace EinBot.Currency.EinEmbed.Extensiosn;

using Discord;
using EinBot.Currency.EinEmbed;
using EinBotDB;
using EinBotDB.DataAccess;
using EinBotDB.Models;

public static partial class EmbedBuilderExtensions
{
    public static EmbedBuilder AddEinEmbedPart(this EmbedBuilder embedBuilder, EinRow einRow, EinEmbedPartsModel partsModel)
    {
        EmbedPartsEnum embedPartType = (EmbedPartsEnum)partsModel.EmbedPartTypesId;

        IEinEmbedPart einEmbedPart;

        switch (embedPartType)
        {
            case EmbedPartsEnum.Title:
                einEmbedPart = new EinEmbedPartTitle(einRow, partsModel);
                break;
            case EmbedPartsEnum.Description:
                einEmbedPart = new EinEmbedPartDescription(einRow, partsModel);
                break;
            case EmbedPartsEnum.Author:
                einEmbedPart = new EinEmbedPartAuthor(einRow, partsModel);
                break;
            case EmbedPartsEnum.Color:
                einEmbedPart = new EinEmbedPartColor(einRow, partsModel);
                break;
            case EmbedPartsEnum.Thumbnail:
                einEmbedPart = new EinEmbedPartThumbnail(einRow, partsModel);
                break;
            case EmbedPartsEnum.Footer:
                einEmbedPart = new EinEmbedPartFooter(einRow, partsModel);
                break;
            case EmbedPartsEnum.Field:
                einEmbedPart = new EinEmbedPartField(einRow, partsModel);
                break;
            case EmbedPartsEnum.Image:
                einEmbedPart = new EinEmbedPartImage(einRow, partsModel);
                break;
            case EmbedPartsEnum.Ephemeral:
                // These need to be handled separetly, as it's what to set the ephemeral property of RespondAsync to..
                return embedBuilder;
            case EmbedPartsEnum.URL:
                einEmbedPart = new EinEmbedPartURL(einRow, partsModel);
                break;
            case EmbedPartsEnum.Attachment:
                // Attachments need to be handled separately.
                return embedBuilder;
            case EmbedPartsEnum.Message:
                // These need to be handled separetly, as it's the message to go along with the RespondAsync.
                return embedBuilder;
            case EmbedPartsEnum.TimeStamp:
                einEmbedPart = new EinEmbedPartTimeStamp(einRow, partsModel);
                break;
            default:
                return embedBuilder;
        }

        return einEmbedPart.AddPartToEmbedBuilder(embedBuilder);
    }
}