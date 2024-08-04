using Telegram.Bot.Types;

namespace NotAPidorBot.Models.GPTReactions;
public static class GptContextStore
{
    private static List<GptContext> _store = new List<GptContext>();

    public static GptContext AddNewContext(string msgText, long messageId)
    {
        var newContext = new GptContext(msgText, messageId);
        _store.Add(newContext);
        return newContext;
    }
    public static GptContext? GetContextByMessage(Message msg)
    {
        if (msg.ReplyToMessage != null)
            foreach (var c in _store)
                if (c.LastMessageId == msg.ReplyToMessage.MessageId)
                {
                    if (c.Created < DateTime.Now.AddDays(-7))
                    {
                        _store.Remove(c);
                        return null;
                    }
                    return c;
                }
        return null;
    }
}