using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NotAPidorBot.Models.TotalContext;

namespace NotAPidorBot.Models.AdminCommands;
public class GetAllContext : AdminCommandBase
{
    public override bool CheckNeedReactionForMessage(Message msg)
    {
        if (msg.From != null && Settings.BotConfiguration.AdminIds.Contains(msg.From.Id) && msg.Chat.Type == ChatType.Private)
        {
            if (!string.IsNullOrWhiteSpace(msg.Text) && msg.Text.Trim().ToLower() == "/getallcontext")
                return true;
        }
        return false;
    }

    public override async Task<Message> SendAsync(ITelegramBotClient bot, ILogger logger, Message msg)
    {
        if (Context.Messages != null && Context.Messages.Count > 0)
        {
            foreach (var message in Context.Messages)
            {
                await bot.SendTextAsync(logger, msg.Chat, Context.PrepareMessageTextToSendToGpt(message));
            }
        }
        else
            await bot.SendReplyTextAsync(logger, msg, "Контекст пуст");
        return null;
    }
}