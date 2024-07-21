namespace NotAPidorBot;
public static class Settings
{
    public static BotConfiguration BotConfiguration { get; private set; }

    public static void Init(BotConfiguration botConfiguration)
    {
        if (botConfiguration is null)
            throw new ArgumentNullException(nameof(botConfiguration), "Parameter cannot be null.");

        BotConfiguration = botConfiguration;
    }
}