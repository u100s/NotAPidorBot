using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NotAPidorBot.Models.TotalContext;

namespace NotAPidorBot.Models.AdminCommands;
public class GetRetellingText : AdminCommandBase
{
    private string commandName = "/getretelling";
    public override string CommandName { get { return commandName; } }

    public override async Task<Message> SendAsync(ITelegramBotClient bot, ILogger logger, Message msg)
    {
        if (!string.IsNullOrWhiteSpace(Context.RetellingText))
            return await bot.SendReplyTextAsync(logger, msg, Context.RetellingText);
        else
            return await bot.SendReplyTextAsync(logger, msg, "Пусто");
    }
}