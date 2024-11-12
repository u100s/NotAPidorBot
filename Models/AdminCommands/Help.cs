using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NotAPidorBot.Models.TotalContext;

namespace NotAPidorBot.Models.AdminCommands;
public class Help : AdminCommandBase
{
    private string commandName = "/help";
    public override string CommandName { get { return commandName; } }

    public override async Task<Message> SendAsync(ITelegramBotClient bot, ILogger logger, Message msg)
    {
        var commands = ReactionHelper.GetAdminReactionsList();
        string message = "";
        foreach (var command in commands)
            message += command.CommandName + Environment.NewLine;

        await bot.SendReplyTextAsync(logger, msg, message.TrimEnd());

        return null;
    }
}