namespace EinBot.Currency.CollectionInteractions;

using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using EinBotDB.DataAccess;
using EinBotDB;
using EinBot.Currency.Extensions;

public partial class CollectionInteractions
{
    [SlashCommand("info", "Usage /collection view {@Role}")]
    public async Task HandleCollectionViewCommand(IRole Role)
    {
        EinTable einTable;

        try
        {
            einTable = _dataAccess.GetEinTable(Role.Id);
        } catch (TableDoesNotExistException e)
        {
            await RespondAsync($"There is no collection associated with the role {Role.Mention}.");
            return;
        }

        Embed embed = new EmbedBuilder().DisplayEinTable(einTable, Role).Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("to-csv", "Usage: /collection to-csv {@Role}")]
    public async Task HandleCollectionToCSVCommand(IRole role)
    {
        try
        {
            var einTable = _dataAccess.GetEinTable(role.Id);
            await RespondAsync($"Loading file...");

            string fileName = $"{einTable.Name}.csv";
            string csvString = einTable.ToCSVString();

            using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvString));

            List<FileAttachment> fileAttachments = new List<FileAttachment>()
            {
                new FileAttachment(memoryStream, fileName)
            };

            await ModifyOriginalResponseAsync(originalResponse =>
                {
                    originalResponse.Attachments = fileAttachments;
                    originalResponse.Content = "File loaded.";
                });

            return;
        } catch (TableDoesNotExistException e)
        {
            await RespondAsync($"There is no collection associated with {role.Mention}.");
            return;
        } catch (ArgumentNullException e)
        {
            await RespondAsync($"Error: {e.Message}.");
            return;
        }
    }
}