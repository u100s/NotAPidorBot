using NotAPidorBot.Characters;

namespace NotAPidorBot.Models.TotalContext;
public class Person
{
    public int SpeakerId { get; private set; }

    public string SpeakerName
    {
        get
        {
            return String.Format("%username_{0}%", SpeakerId);
        }
    }
    public long TelegramUserId { get; private set; }
    public string? UserName { get; private set; }
    public Character Character { get; private set; }
    public string IntroDescription { get; private set; }

    internal Person(int speakerId, long telegramUserId, string? username, Character character)
    {
        SpeakerId = speakerId;
        TelegramUserId = telegramUserId;
        Character = character;
        UserName = username != null ? username : "";

        IntroDescription = Character.CharacterDescription.Replace("%username%", SpeakerName);
    }
}