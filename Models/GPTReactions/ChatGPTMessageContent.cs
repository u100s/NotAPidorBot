namespace NotAPidorBot.Models.GPTReactions;
public class ChatGPTMessageContent
{
    public string type { get; init; } = "text";
    public string text { get; private set; }

    public ChatGPTMessageContent(string msg)
    {
        text = msg;
    }
}
