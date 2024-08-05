namespace NotAPidorBot;
public class CharacterConfiguration
{
    public List<Character> Persons { get; set; }

    public Character GetCharacterByUserId(long userId)
    {
        if (Persons != null)
            foreach (var p in Persons)
                if (p.UserId == userId)
                    return p;
        return null;
    }
}
