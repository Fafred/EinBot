﻿namespace EinBot.Currency.CollectionInteractions;

using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using EinBotDB.DataAccess;
using EinBotDB;

public partial class CollectionInteractions
{
    /// <summary>
    /// Sub-Class for handling CSV related commands.
    /// </summary>
    [Group("csv", "CSV file related commands.")]
    public class CollectionInteractionsCSV : InteractionModuleBase<SocketInteractionContext>
    {
        private IEinDataAccess _dataAccess;

        public CollectionInteractionsCSV(IEinDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        /// <summary>
        /// Creates a table and adds currencies based off the attached CSV file.
        /// </summary>
        /// <param name="role">Role to associate the table with.</param>
        /// <param name="collectionType">The collection type of the table.</param>
        /// <param name="csvFile">A csv file in the format: currency_name, currency_data_type</param>
        /// <returns></returns>
        [SlashCommand("create", "Create a new currency collection from a CSV file.")]
        public async Task HandleCollectionCreateFromCSVCommand(
            IRole role,
            CollectionTypesEnum collectionType,
            IAttachment csvFile)
        {
            await RespondAsync("Not implemented yet.");
            return;
        }

        /// <summary>
        /// Converts a table and its rows to a CSV file and uploads it to the Discord channel.
        /// </summary>
        /// <param name="role">The role associated with the table.</param>
        /// <returns></returns>
        [SlashCommand("get", "Usage: /collection csv get {@Role}")]
        public async Task HandleCollectionToCSVCommand(IRole role)
        {
            try
            {
                var einTable = _dataAccess.GetEinTable(role.Id);
                await RespondAsync($"Converting {role.Mention} to CSV file and uploading.  Please wait.");

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
                        originalResponse.Content = $"Finished converting {role.Mention} to CSV file.";
                    });

                return;
            }
            catch (TableDoesNotExistException e)
            {
                await RespondAsync($"There is no collection associated with {role.Mention}.");
                return;
            }
            catch (ArgumentNullException e)
            {
                await RespondAsync($"Error: {e.Message}.");
                return;
            }
        }
    }
}