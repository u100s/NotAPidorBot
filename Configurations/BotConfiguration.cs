namespace NotAPidorBot.Configurations;
public class BotConfiguration
{
    public string BotToken { get; init; } = default!;
    public string BotUserName { get; init; } = default!;
    public string ChatGptApiKey { get; init; } = default!;
    public string ChatGptModel { get; init; } = default!;
    public int ChatGptMaxTokens { get; init; } = 256;
    public long[] ActiveChatIds { get; init; } = default!;
    public long[] AdminIds { get; init; } = default!;
}
