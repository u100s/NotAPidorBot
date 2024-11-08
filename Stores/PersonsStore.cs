using NotAPidorBot.Characters;
using NotAPidorBot.Models.TotalContext;

namespace NotAPidorBot.Stores;
public static class PersonsStore
{
    public static List<Person> Persons { get; set; } = new List<Person>();

    public static Person GetOrCreatePersonByUser(long userId, string? username)
    {
        Person? result = null;
        foreach (var p in PersonsStore.Persons)
            if (p.TelegramUserId == userId)
                result = p;

        if (result == null)
        {
            var character = Settings.CharacterConfiguration.GetCharacterByUserId(userId);
            if (character == null)
                character = Character.CreateAnonimousCharacterByUserId(userId, username);
            result = new Person(PersonsStore.Persons.Count + 1, userId, username, character);
            PersonsStore.Persons.Add(result);
        }
        return result;
    }

    public static Person? FindPersonBySpeakerId(int speakerId)
    {
        foreach (var p in PersonsStore.Persons)
            if (p.SpeakerId == speakerId)
                return p;
        return null;
    }
}