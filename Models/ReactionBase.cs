using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NotAPidorBot.Models.TotalContext;

namespace NotAPidorBot.Models;
public abstract class ReactionBase
{
    internal virtual string[]? Substrings { get; }
    internal virtual Reply[]? Replies { get; }

    /// <summary>
    /// Сообщение, на которое отвечает бот, обязательно должно быть ответом на раннее сообщение бота
    /// </summary>
    public virtual bool MustBeReply { get; internal set; } = false;
    /// <summary>
    /// Фраза должна полностью соответствовать шаблону
    /// </summary>
    public virtual bool NeedFullMatch { get; internal set; } = false;
    /// <summary>
    /// Вероятность отправки ответа
    /// </summary>
    public virtual int Probability { get; internal set; } = 10;


    public virtual bool CheckNeedReactionForMessage(Message msg)
    {
        if (MustBeReply && !msg.IsItReplyToBotMessage())
        {
            return false;
        }

        if (msg.Type == MessageType.Text && !string.IsNullOrWhiteSpace(msg.Text) && Substrings != null && Substrings.Length > 0)
        {
            var random = new Random();
            if (random.Next(0, 100) < Probability)
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
        {
            var result = await bot.SendReplyTextAsync(logger, msg, reply.Text);
            Context.AddAnswerFromGpt(result.MessageId, reply.Text);
            return result;
        }
        if (reply.Type == MessageType.Sticker)
            return await bot.SendReplyStickerAsync(logger, msg, reply.Text);
        else
            throw new ArgumentException(String.Format("Unknown reply type: {0}", reply.Type));
    }
}