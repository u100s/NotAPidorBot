namespace NotAPidorBot;
public class Character
{
    public long UserId { get; set; }
    public string[] Names { get; set; }
    public string CharacterDescription { get; set; }

    public string GetRandomCharacterName()
    {
        var random = new Random();
        int nameIndex = random.Next(0, Names.Length);
        return Names[nameIndex];
    }
}
