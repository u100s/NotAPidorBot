using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace NotAPidorBot.Models;
public abstract class ReactionBase
{
    internal virtual string[]? Substrings { get; }
    internal virtual Reply[]? Replies { get; }
    public virtual bool MustBeReply { get; internal set; } = false;
    public virtual bool NeedFullMatch { get; internal set; } = false;
    public virtual int Probability { get; internal set; } = 10;


    public virtual bool CheckNeedReactionForMessage(Message msg, float currentRandomScore)
    {
        if (MustBeReply &&
            (msg.ReplyToMessage == null ||
            msg.ReplyToMessage.From == null ||
            !msg.ReplyToMessage.From.IsBot ||
            msg.ReplyToMessage.From.Username != Settings.BotConfiguration.BotUserName))
        {
            return false;
        }

        if (msg.Type == MessageType.Text && !string.IsNullOrWhiteSpace(msg.Text) && Substrings != null && Substrings.Length > 0)
        {
            var random = new Random();
            if (random.Next(0, 100) < Probability * currentRandomScore)
            {
                if (NeedFullMatch)
                    return msg.Text.EqualsAny(Substrings);
                else
                    return msg.Text.ContainsAny(Substrings);
            }
        }
        return false;
    }

    public virtual async Task<Message> SendAsync(ITelegramBotClient bot, ILogger logger, Message msg)
    {
        // Проверка на пустой список
        if (Replies == null || Replies.Length == 0)
        {
            throw new ArgumentException("The list is empty or null.");
        }

        var random = new Random();
        int randomIndex = random.Next(Replies.Length);

        var reply = Replies[randomIndex];
        if (reply.Type == MessageType.Text)
            return await bot.SendReplyTextAsync(logger, msg, reply.Text);
        if (reply.Type == MessageType.Sticker)
            return await bot.SendReplyStickerAsync(logger, msg, reply.Text);
        else
            throw new ArgumentException(String.Format("Unknown reply type: {0}", reply.Type));
    }
}