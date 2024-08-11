namespace NotAPidorBot.Models.ChatGpt;
public class Message
{
    public string role { get; private set; }
    public string type { get; init; } = "text";
    public MessageContent[] content { get; private set; }

    public Message(string msg, bool isAnswerFromGPT)
    {
        content = [new MessageContent(msg)];

        if (isAnswerFromGPT)
            role = "assistant";
        else
            role = "user";
    }
}
