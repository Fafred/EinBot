namespace EinBot.Currency.Extensions;

using Discord;
using EinBotDB.DataAccess;
using System.Collections.Immutable;
using System.Text;

/// <summary>
/// Extensions for Discord.NET's EmbedBuilder class.
/// </summary>
public static class EmbedBuilderExtensions
{
    /// <summary>
    /// Extension to the EmbedBuilder class which takes an EinTable and manipulates an EmbedBuilder object to display it.
    /// </summary>
    /// <param name="eb">This embed builder.</param>
    /// <param name="eTable">The EinTable to display.</param>
    /// <param name="role">The role of the table.  This should match the role associated with the EinTable, but cannot be not enforced.</param>
    /// <returns></returns>
    public static EmbedBuilder DisplayEinTable(this EmbedBuilder eb, EinTable eTable, IRole role)
    {
        StringBuilder strBuilder = new StringBuilder();

        strBuilder.Append("This is a ");
        CollectionTypesEnum collectionType = (CollectionTypesEnum)eTable.CollectionTypeId;

        switch (collectionType)
        {
            case CollectionTypesEnum.PerKey:
                strBuilder.Append("**PerKey** collection, meaning there can be multiple instances of it per user.");
                break;
            case CollectionTypesEnum.PerUser:
                strBuilder.Append("**PerUser** collection, meaning there can only be one instance of it per user.");
                break;
            case CollectionTypesEnum.PerRole:
                strBuilder.Append("**PerRole** collection, meaning there is only one instance of it.");
                break;
        }

        string collectionTypeStr = strBuilder.ToString();
        strBuilder.Clear();

        eb.WithColor(role.Color);
        eb.WithTitle(role.Name);
        eb.WithDescription($"{collectionTypeStr}\n\nCurrencies in this collection are displayed by the name of the currency, followed by the data type.\n\n:white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square:\n\n{role.Mention} has the following currencies:\n\n");

        var rowKeys = eTable.ColumnDataTypes.Keys.ToList();
        rowKeys.Sort();

        foreach (var key in rowKeys)
        {
            eb.AddField(key, eTable.ColumnDataTypes[key], inline: true);
        }

        return eb;
    }
}
