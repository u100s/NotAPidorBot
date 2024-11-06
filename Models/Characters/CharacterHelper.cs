using NotAPidorBot.Stores;
using System.Text.RegularExpressions;

namespace NotAPidorBot.Characters;
public static class CharacterHelper
{
    public static string AnonimyzeText(this string textToAnonimyze)
    {
        // Меняем упоминания с тегами юзеров, типа @u100s на %username_1%, чтобы в ChatGPT отправлять меньше приватных данных
        foreach (var p in PersonsStore.Persons)
        {
            if (!string.IsNullOrWhiteSpace(p.Character.UserLogin))
            {
                textToAnonimyze = Regex.Replace(textToAnonimyze, "@" + p.Character.UserLogin, p.SpeakerName, RegexOptions.IgnoreCase);
            }
            if (p.Character != null)
            {
                if (p.Character.Names != null)
                    foreach (var name in p.Character.Names)
                    {
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            textToAnonimyze = Regex.Replace(textToAnonimyze, name, p.SpeakerName, RegexOptions.IgnoreCase);
                        }
                    }
                if (p.Character.SecondFormNames != null)
                    foreach (var name in p.Character.SecondFormNames)
                    {
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            textToAnonimyze = Regex.Replace(textToAnonimyze, name, p.SpeakerName, RegexOptions.IgnoreCase);
                        }
                    }
            }
        }
        return textToAnonimyze;
    }

    public static string DeanonimyzeText(this string text)
    {
        string result = text;
        foreach (var p in PersonsStore.Persons)
            result = result.Replace(p.SpeakerName, p.Character.GetRandomCharacterName());

        return result;
    }

    public static string GetCharactersDecriptions()
    {
        var charactersDesciption = "В чате есть:";
        if (PersonsStore.Persons.Count < 1)
            charactersDesciption += "ты один";
        else
        {
            foreach (var p in PersonsStore.Persons)
                charactersDesciption += " " + p.SpeakerId.ToString() + ". " + p.IntroDescription;
        }

        return charactersDesciption;
    }
}
