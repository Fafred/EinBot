namespace EinBot.General.Models;

using Discord;
using Discord.Interactions;
using EinBotDB.DataAccess;

public partial class GeneralInteractionCommands : InteractionModuleBase<SocketInteractionContext>
{
    /// <summary>
    /// Displays the id and mention code of the given mentionable (user/role/channel)
    /// </summary>
    /// <param name="user">The user to get the id of.</param>
    /// <param name="role">The role to get the id of.</param>
    /// <param name="channel">The channel to get the id of.</param>
    /// <returns></returns>
    [SlashCommand("id", "Returns the ID of the target.")]
    public async Task HandleGeneralIdCommand(IUser? user = null, IRole? role = null, IChannel? channel = null)
    {
        if (user is null && role is null && channel is null)
        {
            await RespondAsync("Nothing to id.  Give it a user, role, or channel in the appropriate prompt.", ephemeral: true);
            return;
        }

        string mention;
        string type;
        string id;

        if (user is not null)
        {
            mention = user.Mention;
            type = "User";
            id = user.Id.ToString();
        } else if (role is not null)
        {
            mention = role.Mention;
            type = "Role";
            id = role.Id.ToString();
        } else
        {
            id = channel.Id.ToString();
            type = "Channel";
            mention = $"<#{id}>";
        }

        await RespondAsync($"{type}: {mention}\tid: `{id}`\tcode: `{mention}`", ephemeral: true);
        return;
    }
}