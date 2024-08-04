using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace NotAPidorBot.Models.SimpleReactions;
public class ForPidorsReaction : ReactionBase
{
    public override int Probability { get; internal set; } = 10;

    private Reply[] _replies = new[]
    {
        new Reply("для пидоров"),
        new Reply("— это грустно"),
        new Reply("— это печально"),
        new Reply("для лохов"),
        new Reply("— это жиза"),
        new Reply("чисто по кайфу"),
        new Reply("для задротов"),
        new Reply("— это топчик"),
        new Reply("для мамкиных экспертов"),
        new Reply("— это огонь"),
        new Reply("— это нечто"),
        new Reply(". Для тех, кто в теме"),
        new Reply("— это просто шедевр"),
        new Reply("— это просто пушка"),
        new Reply("для эстетов"),
        new Reply("— это лютый зашквар"),
        new Reply("для тех, кто шарит"),
        new Reply("для самых крутых"),
        new Reply("— это просто бомба"),
        new Reply("— это просто жесть"),
        new Reply(". Для истинных ценителей")
    };
    internal override Reply[] Replies { get { return _replies; } }
    private string _wordForPidors = "";

    public override bool CheckNeedReactionForMessage(Message msg, float currentRandomScore)
    {
        // Не отвечаем на сообщения-ответы боту, т.к. там может быть команда
        if (msg.ReplyToMessage != null &&
            msg.ReplyToMessage.From != null &&
            msg.ReplyToMessage.From.IsBot &&
            msg.ReplyToMessage.From.Username == "NotAPidorBot")
        {
            return false;
        }

        if (msg.Type == MessageType.Text && !string.IsNullOrWhiteSpace(msg.Text) && msg.Text.Length < 100)
        {
            var random = new Random();
            if (random.Next(0, 100) < Probability * currentRandomScore)
            {
                var nouns = msg.Text.ExtractNouns();
                if (nouns != null && nouns.Length > 0)
                {
                    _wordForPidors = nouns[random.Next(nouns.Length)];
                    return true;
                }
            }
        }
        return false;
    }

    public override async Task<Message> SendAsync(ITelegramBotClient bot, ILogger logger, Message msg)
    {
        // Проверка на пустой список
        if (Replies == null || Replies.Length == 0)
        {
            throw new ArgumentException("The list is empty or null.");
        }

        if (msg == null || String.IsNullOrWhiteSpace(msg.Text))
        {
            throw new ArgumentException("Original message is empty or null.");
        }

        if (!String.IsNullOrWhiteSpace(_wordForPidors))
        {
            var random = new Random();
            int randomReplyIndex = random.Next(Replies.Length);
            var reply = Replies[randomReplyIndex];
            if (reply.Type == MessageType.Text)
                return await bot.SendReplyTextAsync(logger, msg, String.Format("{0} {1}", _wordForPidors, reply.Text));
            if (reply.Type == MessageType.Sticker)
                return await bot.SendReplyStickerAsync(logger, msg, reply.Text);
            else
                throw new ArgumentException(String.Format("Unknown reply type: {0}", reply.Type));
        }
        return null;
    }
}