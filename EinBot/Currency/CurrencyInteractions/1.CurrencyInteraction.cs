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


}
