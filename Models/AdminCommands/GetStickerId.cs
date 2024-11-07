using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace NotAPidorBot.Models.AdminCommands;
public class GetStickerId : AdminCommandBase
{
    public override bool CheckNeedReactionForMessage(Message msg)
    {
        var result = base.CheckNeedReactionForMessage(msg);
        return result && msg.Type == MessageType.Sticker;
    }

    public override async Task<Message> SendAsync(ITelegramBotClient bot, ILogger logger, Message msg)
    {
        return await bot.SendTextMessageAsync(msg.Chat, msg.Type.ToString() + ' ' + msg.Sticker.FileId, parseMode: ParseMode.Html, replyParameters: new ReplyParameters() { MessageId = msg.MessageId }, replyMarkup: new ReplyKeyboardRemove());
    }
}