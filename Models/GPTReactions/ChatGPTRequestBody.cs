using System.Collections.Generic;
using System.Linq;

namespace NotAPidorBot.Models.GPTReactions;
public class ChatGPTRequestBody
{

    public string model { get; init; } = "gpt-4o";
    public int max_tokens { get; init; } = Settings.BotConfiguration.ChatGptMaxTokens;
    public ChatGPTMessage[] messages { get; private set; }
    public int temperature { get; init; } = 1;

    public ChatGPTRequestBody(string msg)
    {
        messages = [new ChatGPTMessage(msg, false)];
    }
    public ChatGPTRequestBody(GptContext context)
    {
        messages = context.Messages.Select(m => new ChatGPTMessage(m.MessageText, m.IsAnswerFromGPT)).ToArray();
    }
}
