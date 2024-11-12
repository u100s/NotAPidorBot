using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NotAPidorBot.Models.TotalContext;

namespace NotAPidorBot.Models.AdminCommands;
public class ClearContext : AdminCommandBase
{
    private string commandName = "/clearcontext";
    public override string CommandName { get { return commandName; } }

    public override async Task<Message> SendAsync(ITelegramBotClient bot, ILogger logger, Message msg)
    {
        if (Context.Messages != null && Context.Messages.Count > 0)
        {
            Context.Messages.Clear();
            await bot.SendReplyTextAsync(logger, msg, "Контекст почищен");
        }
        return null;
    }
}