using Discord.Interactions;
using Discord;
using EinBot.Currency.CurrencyInteractions.Exceptions;
using EinBotDB;
using EinBotDB.DataAccess;
using EinBotDB.Models;
using CsvHelper;
using System.Text;

public partial class CurrencyInteractions
{
    [Group("csv", "CSV file related commands.")]
    public class CurrencyInteractionCSV : InteractionModuleBase<SocketInteractionContext>
    {
        private IEinDataAccess _dataAccess;

        public CurrencyInteractionCSV(IEinDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [SlashCommand("update", "Update a collection with values from a CSV file.")]
        public async Task HandleCurrencyUpdateFromCSVCommand(
            IRole role,
            IAttachment csvFile)
        {
            TableDefinitionsModel tableDefinition;

            try
            {
                tableDefinition = _dataAccess.GetTable(roleId: role.Id);
            }
            catch (TableDoesNotExistException)
            {
                await RespondAsync($"```diff\n-[Failure]-\n```No collection assigned to role {role.Mention}.");
                return;
            }

            var tableId = tableDefinition.Id;

            await RespondAsync($"Attempting to parse {csvFile.Filename}.");

            List<Dictionary<string, string>> dictList = new();
            string[] headers = Array.Empty<string>();

            // Attempt to read in the CSV file.
            try
            {
                using var httpClient = new HttpClient();
                using var stream = httpClient.GetStreamAsync(csvFile.Url).Result;
                using var streamReader = new StreamReader(stream);
                using var csvReader = new CsvReader(streamReader, System.Globalization.CultureInfo.CurrentCulture);

                csvReader.Read();
                csvReader.ReadHeader();
                headers = csvReader.HeaderRecord ?? Array.Empty<string>();


                while(csvReader.Read())
                {
                    Dictionary<string, string> record = new();

                    foreach (var header in headers)
                    {
                        var field = csvReader.GetField(header);
                        record.Add(header, field);
                    }
                    dictList.Add(record);
                }
            }
            catch (Exception e)
            {
                await FollowupAsync($"```diff\n-[Failure]-\n```\n{e.Message}");
            }

            // Add or update the rows.
            StringBuilder sb = new();
            sb.Append("```diff\n                ");
            foreach (var header in headers)
            {
                sb.Append($"{header}, ");
            }
            sb.AppendLine();

            int line = 1;
            foreach (var record in dictList)
            {
                try
                {
                    if (!record.ContainsKey("Key")) throw new Exception("No `Key` field.");
                    var key = record["Key"];

                    _dataAccess.AddOrUpdateRow(key, record, tableId: tableId);
                    sb.Append($"+[{line}] [SUCCESS]+ ");
                    foreach(var recKey in record.Keys)
                    {
                        sb.Append($"{record[recKey]}, ");
                    }
                    sb.AppendLine();
                }
                catch (Exception e)
                {
                    sb.AppendLine($"-[{line}] [FAILURE]- {e.ToString()}");
                }

                ++line;
            }
            sb.Append("```");

            await FollowupAsync($"Add/Update currency {role.Mention}:\n{sb}");
        }
    }
}