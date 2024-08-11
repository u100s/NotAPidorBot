using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using NotAPidorBot.Helpers;

namespace NotAPidorBot.Models.GPTReactions;
public class GptMessage
{
    public int SpeakerId { get; private set; }
    public long MessageId { get; private set; }
    public string MessageText { get; private set; }
    public bool IsAnswerFromGPT { get; private set; }

    public GptMessage(long messageId, string messageText, bool isAnswerFromGPT)
    {
        MessageId = messageId;
        MessageText = messageText;
        IsAnswerFromGPT = isAnswerFromGPT;
    }
}