namespace EinBot.Currency.EinEmbed;

using System.Text;

using Discord;
using EinBotDB;
using EinBotDB.DataAccess;
using EinBotDB.Models;

internal abstract class EinEmbedPartBase : IEinEmbedPart
{
    internal EinRow _einRow;
    internal EmbedPartsEnum _partsType;
    internal string _data01;
    internal string _data02;
    internal string _data03;

    public EinEmbedPartBase(EinRow einRow, EinEmbedPartsModel partModel)
    {
        _einRow = einRow;
        _partsType = (EmbedPartsEnum)partModel.EmbedPartTypesId;
        _data01 = string.IsNullOrEmpty(partModel.Data01) ? "" : Replacer(partModel.Data01);
        _data02 = string.IsNullOrEmpty(partModel.Data02) ? "" : Replacer(partModel.Data02);
        _data03 = string.IsNullOrEmpty(partModel.Data03) ? "" : Replacer(partModel.Data03);


    }

    public abstract EmbedBuilder AddPartToEmbedBuilder(EmbedBuilder embedBuilder);

    private string Replacer(string data)
    {
        if (string.IsNullOrEmpty(data)) return "";
        if (!data.Contains('{') && !data.Contains("\\n") && !data.Contains("\\t")) return data;

        int count = 0;

        foreach (char c in data.ToCharArray())
        {
            if (c.Equals('{')) ++count;
        }

        foreach (var key in _einRow.Columns.Keys)
        {
            if (count < 1) break;

            var columnInfo = _einRow[key];

            var searchTerm = $"{{{key}}}";
            string replaceTerm;

            if (!columnInfo.IsColumnList)
            {
                var columnData = (columnInfo.Data is null ? "[NULL]" : (string)columnInfo.Data);

                replaceTerm = columnInfo.DataType switch
                {
                    DataTypesEnum.UserId => $"<@{columnData}>",
                    DataTypesEnum.ChannelId => $"<#{columnData}>",
                    DataTypesEnum.RoleId => $"<@&{columnData}>",
                    _ => $"{columnData}",
                };


                var newData = data.Replace(searchTerm, replaceTerm);
                if (!newData.Equals(data))
                {
                    --count;
                    data = newData;
                }
            }
        }

        data = data.Replace("\\n", "\n").Replace("\\t", "\t");

        return data;
    }
}