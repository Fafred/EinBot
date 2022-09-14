namespace EinBot.Currency.CurrencyInteractions;

using Discord.Interactions;
using EinBotDB.DataAccess;

[Group("currency", "Commands for reading and updating currencies.")]
public partial class CurrencyInteractions : InteractionModuleBase<SocketInteractionContext>
{
    IEinDataAccess _dataAccess;

    public CurrencyInteractions(IEinDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    private async Task RespondSuccessAsync(string message, bool ephemeral = false)
    {
        await RespondAsync($"```diff\n+[Success]+\n```\n{message}", ephemeral: ephemeral);
    }

    private async Task RespondFailureAsync(string message, bool ephemeral = false)
    {
        await RespondAsync($"```diff\n-[Failure]-\n```\n{message}", ephemeral: ephemeral);
    }
}
