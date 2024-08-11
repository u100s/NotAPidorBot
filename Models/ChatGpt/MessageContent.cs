namespace NotAPidorBot.Models.ChatGpt;
public class MessageContent
{
    public string type { get; init; } = "text";
    public string text { get; private set; }

    public MessageContent(string msg)
    {
        text = msg;
    }
}
