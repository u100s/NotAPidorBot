using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NotAPidorBot.Models.TotalContext;

namespace NotAPidorBot.Models.AdminCommands;
public abstract class AdminCommandBase : ReactionBase
{
    public override bool CheckNeedReactionForMessage(Message msg)
    {
        return msg.From != null && Settings.BotConfiguration.AdminIds.Contains(msg.From.Id) && msg.Chat.Type == ChatType.Private;
    }

    public override abstract Task<Message> SendAsync(ITelegramBotClient bot, ILogger logger, Message msg);
}