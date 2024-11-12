using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NotAPidorBot.Models.TotalContext;

namespace NotAPidorBot.Models.AdminCommands;
public abstract class AdminCommandBase : ReactionBase
{

    public abstract string CommandName { get; }
    public override bool CheckNeedReactionForMessage(Message msg)
    {
        if (msg.From != null && Settings.BotConfiguration.AdminIds.Contains(msg.From.Id) && msg.Chat.Type == ChatType.Private)
        {
            if (!string.IsNullOrWhiteSpace(msg.Text) && msg.Text.Trim().ToLower().Contains(CommandName))
                return true;
        }
        return false;
    }

    public override abstract Task<Message> SendAsync(ITelegramBotClient bot, ILogger logger, Message msg);
}