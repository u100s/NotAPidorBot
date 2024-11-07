using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NotAPidorBot.Models.TotalContext;

namespace NotAPidorBot.Models.AdminCommands;
public class GetRetellingText : AdminCommandBase
{
    public override bool CheckNeedReactionForMessage(Message msg)
    {
        if (msg.From != null && Settings.BotConfiguration.AdminIds.Contains(msg.From.Id) && msg.Chat.Type == ChatType.Private)
        {
            if (!string.IsNullOrWhiteSpace(msg.Text) && msg.Text.Trim().ToLower() == "/getretelling")
                return true;
        }
        return false;
    }

    public override async Task<Message> SendAsync(ITelegramBotClient bot, ILogger logger, Message msg)
    {
        if (!string.IsNullOrWhiteSpace(Context.RetellingText))
            return await bot.SendReplyTextAsync(logger, msg, Context.RetellingText);
        else
            return await bot.SendReplyTextAsync(logger, msg, "Пусто");
    }
}