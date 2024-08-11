using Telegram.Bot.Types;
using NotAPidorBot.Models.ChatGpt;

namespace NotAPidorBot.Models.GPTReactions;
public class GptContext
{
    public DateTime Created { get; private set; }
    public long OriginalMessageId { get; private set; }
    public long LastMessageId { get; private set; }

    public List<GptMessage> Messages { get; set; }

    public GptContext(string msgText, long messageId)
    {
        Created = DateTime.Now;
        OriginalMessageId = messageId;
        Messages = [new GptMessage(messageId, msgText, false)];
        LastMessageId = messageId;
    }

    public GptContext(Telegram.Bot.Types.Message msg)
    {
        Created = DateTime.Now;
        OriginalMessageId = msg.MessageId;
        Messages = new List<GptMessage>();
        AddMessage(msg);
    }

    public void AddMessage(Telegram.Bot.Types.Message msg, bool isAnswerFromGPT = false)
    {
        if (!string.IsNullOrWhiteSpace(msg.Text))
        {
            Messages.Add(new GptMessage(msg.MessageId, msg.Text, isAnswerFromGPT));
            LastMessageId = msg.MessageId;
        }
    }

    public RequestBody GetRequestBody()
    {
        var result = new RequestBody();
        result.messages = Messages.Select(m => new ChatGpt.Message(m.MessageText, m.IsAnswerFromGPT)).ToArray();
        return result;
    }
}