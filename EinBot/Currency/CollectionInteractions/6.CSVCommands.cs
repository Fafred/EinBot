namespace EinBot.Currency.CollectionInteractions;

using CsvHelper;
using Discord;
using Discord.Interactions;
using EinBotDB;
using EinBotDB.DataAccess;
using EinBotDB.Models;
using System.Text;
using System.Threading.Tasks;

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
            TableDefinitionsModel tableDefinition;

            try
            {
                tableDefinition = _dataAccess.CreateTable(role.Name, collectionType, role.Id);

                if (tableDefinition is null) throw new NullReferenceException();
            }
            catch (TableAlreadyExistsException)
            {
                await RespondAsync($"`[Failure]` There is already a collection assigned to role {role.Mention}.");
                return;
            }
            catch (NullReferenceException)
            {
                await RespondAsync($"`[Failure]` Unable to create collection.");
                return;
            }

            await RespondAsync($"`[Success]` Collection {role.Mention} has been created as a {(CollectionTypesEnum)tableDefinition.CollectionTypeId} collection.\n\nPlease wait while attempting to create currencies from the following file:\n```\nFilename: {csvFile.Filename}\nContent type: {csvFile.ContentType}\nProxy URL: {csvFile.Url}\nSize: {csvFile.Size}\n```");

            try
            {
                using var httpClient = new HttpClient();
                using var stream = httpClient.GetStreamAsync(csvFile.Url).Result;
                using var streamReader = new StreamReader(stream);
                using var csvReader = new CsvReader(streamReader, System.Globalization.CultureInfo.CurrentCulture);

                var anonRecord = new
                {
                    CurrencyName = "",
                    DataType = "",
                };


                List<string> resultList = new List<string>();
                int counter = 1;
                foreach (var record in csvReader.GetRecords(anonRecord))
                {
                    try
                    {
                        if (record.CurrencyName is null)
                        {
                            resultList.Add($"-[Row {counter++:D3}]\t{record} NOT ADDED. CURRENCY NAME CANNOT BE NULL.");
                            continue;
                        }
                        else if (record.DataType is null)
                        {
                            resultList.Add($"-[Row {counter++:D3}]\t{record} NOT ADDED. DATA TYPE CANNOT BE NULL.");
                            continue;
                        }

                        var columnName = record.CurrencyName;
                        var dataTypeId = _dataAccess.GetDataTypeId(record.DataType);

                        if (dataTypeId is null)
                        {
                            resultList.Add($"-[Row  {counter++:D3} ]\t{record} NOT ADDED.  INVALID DATA TYPE.");
                            continue;
                        }

                        _dataAccess.CreateColumn(columnName, (DataTypesEnum)dataTypeId, tableId: tableDefinition.Id);
                        resultList.Add($"+[Row {counter++:D3}]\t{record.CurrencyName} {{{record.DataType}}} has been added to {role.Name}.");
                    }
                    catch (ColumnAlreadyExistsException)
                    {
                        resultList.Add($"-[Row  {counter++:D3} ]\t{record} NOT ADDED.  CURRENCY ALREADY EXISTS IN THIS COLLECTION.");
                        continue;
                    }
                    catch (InvalidNameException)
                    {
                        resultList.Add($"-[Row  {counter++:D3} ]\t{record} NOT ADDED.  INVALID CURRENCY NAME.");
                        continue;
                    }
                }

                StringBuilder sb = new();
                sb.AppendLine("```diff");

                foreach (var line in resultList)
                {
                    if (sb.Length + line.Length < 1990)
                    {
                        sb.AppendLine(line);
                    }
                    else
                    {
                        sb.AppendLine("```");
                        await FollowupAsync(sb.ToString());
                        sb.Clear();
                        sb.AppendLine("```diff");
                        sb.AppendLine(line);
                    }
                }

                sb.AppendLine("```");

                await FollowupAsync(sb.ToString());

            }
            catch (Exception e)
            {
                await FollowupAsync($"`[Failure]` Something went wrong. Namely:\n```\n{e.Message}\n```");
                return;
            }
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
                var einTable = _dataAccess.GetEinTable(roleId: role.Id);
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
            catch (TableDoesNotExistException)
            {
                await RespondAsync($"`[Failure]` There is no collection associated with {role.Mention}.");
                return;
            }
            catch (ArgumentNullException e)
            {
                await RespondAsync($"`[Failure]` Error: {e.Message}.");
                return;
            }
        }
    }
}