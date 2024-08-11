using System.Collections.Generic;
using System.Linq;

namespace NotAPidorBot.Models.ChatGpt;
public class RequestBody
{
    public string model { get; init; } = "gpt-4o";
    public int max_tokens { get; init; } = Settings.BotConfiguration.ChatGptMaxTokens;
    public Message[] messages { get; set; }
    public int temperature { get; init; } = 1;
}
