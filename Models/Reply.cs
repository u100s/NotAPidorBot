using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models;
public class Reply
{
    public string Text { get; private set; }
    public MessageType Type { get; private set; }

    public Reply(string text, MessageType type = MessageType.Text)
    {
        Text = text;
        Type = type;
    }
}