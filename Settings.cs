namespace NotAPidorBot;
using NotAPidorBot.Configurations;
public static class Settings
{
    public static BotConfiguration BotConfiguration { get; private set; }

    public static CharacterConfiguration CharacterConfiguration { get; private set; }

    public static ContextConfiguration ContextConfiguration { get; private set; }

    public static void Init(BotConfiguration botConfiguration, CharacterConfiguration characterConfiguration, ContextConfiguration contextConfiguration)
    {
        ArgumentNullException.ThrowIfNull(botConfiguration);
        ArgumentNullException.ThrowIfNull(characterConfiguration);
        ArgumentNullException.ThrowIfNull(contextConfiguration);

        BotConfiguration = botConfiguration;
        CharacterConfiguration = characterConfiguration;
        ContextConfiguration = contextConfiguration;
    }
}