namespace NotAPidorBot;
public class CharacterConfiguration
{
    public string InitialMessage { get; set; }
    public string LastMessageCondition { get; set; }
    public List<Character> Characters { get; set; }

    public Character? GetCharacterByUserId(long userId)
    {
        if (Characters != null)
            foreach (var p in Characters)
                if (p.UserId == userId)
                    return p;
        return null;
    }
}
