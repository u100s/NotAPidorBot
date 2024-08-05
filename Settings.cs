namespace NotAPidorBot;
public static class Settings
{
    public static BotConfiguration BotConfiguration { get; private set; }

    public static CharacterConfiguration CharacterConfiguration { get; private set; }

    public static void Init(BotConfiguration botConfiguration, CharacterConfiguration characterConfiguration)
    {
        if (botConfiguration is null)
            throw new ArgumentNullException(nameof(botConfiguration), "Parameter cannot be null.");
        if (characterConfiguration is null)
            throw new ArgumentNullException(nameof(characterConfiguration), "Parameter cannot be null.");

        BotConfiguration = botConfiguration;
        CharacterConfiguration = characterConfiguration;
    }
}