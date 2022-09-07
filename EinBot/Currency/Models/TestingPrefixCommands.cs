namespace EinBot.General.Models;

using DataAccess.Data;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

public class TestingPrefixCommands : ModuleBase<SocketCommandContext>
{

    private IServiceProvider _serviceProvider = Program.ServiceProvider;

    public TestingPrefixCommands(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [Command("Datatypes")]
    public async Task HandleDataTypes()
    {
        var typesData = _serviceProvider.GetRequiredService<ITypesData>();

        Console.WriteLine("Here");

        StringBuilder builder = new StringBuilder();

        foreach(var val in typesData.GetDataTypes().Result)
        {
            builder.AppendLine(val.Type);
        }

        await ReplyAsync(builder.ToString());
    }
}
