namespace EinBot.General.Models;

using Discord.Commands;

public class CommandModule : ModuleBase<SocketCommandContext>
{
    [Command("hello")]
    [Alias("hi", "greetings")]
    [Summary("Babbies first command.")]
    public async Task HandleHelloCommand() => await ReplyAsync("World.");

    [Command("break")]
    [Alias("br", "brk")]
    [Summary("Puts a visible seperator into the chat.")]
    public async Task HandleBreakCommand() => await ReplyAsync("```diff\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n+ + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + +\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n```");
}
