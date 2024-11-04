namespace NotAPidorBot;
public class Character
{
    public long UserId { get; set; }
    public string? UserLogin { get; set; }
    public string[] Names { get; set; }
    public string CharacterDescription { get; set; }

    public string GetRandomCharacterName()
    {
        var random = new Random();
        int nameIndex = random.Next(0, Names.Length);
        return Names[nameIndex];
    }

    public static Character CreateAnonimousCharacterByUserId(long userId, string? login)
    {
        return new Character
        {
            UserId = userId,
            UserLogin = login,
            Names = ["дружище", "брат", "братан", "братишка", "чувак"],
            CharacterDescription = "%username%, один из твоих знакомых в чате, но близко ты его не знаешь."
        };
        ;
    }
}
