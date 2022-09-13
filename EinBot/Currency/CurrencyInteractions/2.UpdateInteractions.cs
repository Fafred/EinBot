namespace EinBot.Currency.CurrencyInteractions;

using Discord;
using Discord.Interactions;
using EinBotDB.DataAccess;
using System.Text;

public partial class CurrencyInteractions
{
    [SlashCommand("set", "Sets the value of a currency.")]
    public async Task HandleCurrencySetCommand(IRole role, string currencyName, string setValue, IUser? user = null, string? key = null)
    {

        await RespondAsync($"Setting {currencyName} of {role.Mention} to [{setValue}].\nUser: {user?.Mention ?? "NULL"}\nKey: {key ?? "NULL"}");
    }
}
