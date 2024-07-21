namespace NotAPidorBot;
public class BotConfiguration
{
    public string BotToken { get; init; } = default!;
    public string BotUserName { get; init; } = default!;
    public string ChatGptApiKey { get; init; } = default!;
    public long[] ActiveChatIds { get; init; } = default!;
    public long[] AdminIds { get; init; } = default!;
}
