namespace EinBot.Currency.CurrencyInteractions;

using Discord;
using Discord.Interactions;
using EinBot.Currency.CurrencyInteractions.Exceptions;
using EinBotDB;
using EinBotDB.DataAccess;
using EinBotDB.Models;

public partial class CurrencyInteractions
{
    [SlashCommand("view", "Displays the value of the currency.")]
    public async Task HandleCurrencyViewCommand(IRole role, string currencyName, string? key = null, IUser? user = null)
    {

    }
}
