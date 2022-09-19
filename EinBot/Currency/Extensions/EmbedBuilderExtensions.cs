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

        strBuilder.AppendLine("\n");

        strBuilder.AppendLine($"There are {eTable.ColumnDataTypes.Count} currencies in this collection.");
        strBuilder.AppendLine($"There are {eTable.Rows.Count} instances of this collection.");
        strBuilder.AppendLine("\n\n:white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square:");

        eb.WithColor(role.Color);
        eb.WithTitle(role.Name);
        eb.WithDescription($"{strBuilder}");
        return eb;
    }

    public static Embed[] GetEinTableInstances(EinTable eTable, IRole role)
    {
        Func<string, string> mentionFunc = (CollectionTypesEnum)eTable.CollectionTypeId switch
        {
            CollectionTypesEnum.PerUser => (str => $"<@{str}>"),
            CollectionTypesEnum.PerRole => (str => $"<@&{str}>"),
            _ => (str => str),
        };

        var rowKeys = (from row in eTable.Rows
                       select row.Key).ToList();

        rowKeys.Sort();

        EmbedBuilder builder = new();
        List<Embed> embedList = new();
        int counter = 0;

        foreach (var key in rowKeys)
        {
            if (counter % 25 == 0)
            {
                if (counter > 0) embedList.Add(builder.Build());

                builder = new();

                builder.WithTitle($"{eTable.Name} Instances");
                builder.WithFooter($"Page {(counter / 25) + 1} of {(rowKeys.Count / 25) + 1}.");
                builder.WithColor(role.Color);

                if (counter == 0) builder.WithDescription($"{role.Mention} has the following instances:\n\n:white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square:\n\n");
            }

            builder.AddField("_ _", mentionFunc(key), inline: true);
            ++counter;
        }

        embedList.Add(builder.Build());

        return embedList.ToArray();
    }

    public static Embed[] GetEinTableFields(EinTable eTable, IRole role)
    {
        var rowKeys = eTable.ColumnDataTypes.Keys.ToList();

        rowKeys.Sort();

        EmbedBuilder builder = new();
        List<Embed> embedList = new();
        int counter = 0;

        foreach (var key in rowKeys)
        {
            if (counter % 25 == 0)
            {
                if (counter > 0) embedList.Add(builder.Build());

                builder = new();

                builder.WithTitle($"{eTable.Name} Currencies");
                builder.WithFooter($"Page {(counter / 25) + 1} of {(rowKeys.Count / 25) + 1}.");
                builder.WithColor(role.Color);

                if (counter == 0) builder.WithDescription($"Currencies in this collection are displayed by the name of the currency, followed by the data type.\n\n{role.Mention} has the following currencies:\n\n:white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square::white_small_square:\n\n");
            }

            builder.AddField(key, eTable.ColumnDataTypes[key], inline: true);
            ++counter;
        }

        embedList.Add(builder.Build());

        return embedList.ToArray();
    }
}
