namespace NotAPidorBot.Models.GPTReactions;
public class ChatGPTMessage
{
    public string role { get; private set; }
    public string type { get; init; } = "text";
    public ChatGPTMessageContent[] content { get; private set; }

    public ChatGPTMessage(string msg, bool isAnswerFromGPT)
    {
        content = [new ChatGPTMessageContent(msg)];

        if (isAnswerFromGPT)
            role = "assistant";
        else
            role = "user";
    }
}
