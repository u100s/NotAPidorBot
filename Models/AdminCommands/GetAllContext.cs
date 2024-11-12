using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NotAPidorBot.Models.TotalContext;

namespace NotAPidorBot.Models.AdminCommands;
public class GetAllContext : AdminCommandBase
{
    private string commandName = "/getallcontext";
    public override string CommandName { get { return commandName; } }

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