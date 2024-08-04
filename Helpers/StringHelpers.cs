using System;
using System.Text;
using System.Text.RegularExpressions;

namespace NotAPidorBot.Helpers;

public static class StringHelpers
{
    public static bool ContainsAny(this string mainString, params string[] substrings)
    {
        if (string.IsNullOrWhiteSpace(mainString) || substrings == null || substrings.Length == 0)
            return false;

        string normalizedMainString = mainString.Replace(" ", "").ToLower();

        foreach (string substring in substrings)
        {
            if (substring == null)
                continue;

            string normalizedSubstring = substring.Replace(" ", "").ToLower();

            if (normalizedMainString.Contains(normalizedSubstring))
                return true;
        }

        return false;
    }

    public static bool EqualsAny(this string mainString, params string[] substrings)
    {
        if (string.IsNullOrWhiteSpace(mainString) || substrings == null || substrings.Length == 0)
            return false;

        string normalizedMainString = mainString.Replace(" ", "").ToLower();

        foreach (string substring in substrings)
        {
            if (substring == null)
                continue;

            string normalizedSubstring = substring.Replace(" ", "").ToLower();

            if (normalizedMainString == normalizedSubstring)
                return true;
        }

        return false;
    }

    public static string RemoveNonAlphabeticCharacters(this string input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        // Регулярное выражение для удаления всех символов, кроме русских и английских букв
        string pattern = @"[^a-zA-Zа-яА-ЯёЁ]";

        // Замена всех символов, не соответствующих шаблону, на пустую строку
        string result = Regex.Replace(input, pattern, string.Empty);

        return result;
    }

    public static string[] ExtractNouns(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Input string cannot be null or empty.", nameof(input));
        }

        // Типичные окончания существительных в русском языке
        string[] nounEndings = { "ия", "ок", "ек", "ёк", "ец", "ка", "ко", "ца", "ца", "ша", "ща", "чка", "чек", "шка", "щик", "ник", "чик", "чик", "ик", "ок", "ек", "ёк", "ец" };

        // Разделяем строку на слова, удаляя знаки препинания
        var words = input.Split(new[] { ' ', '.', ',', '!', '?', ';', ':', '-', '(', ')', '[', ']', '{', '}', '"' }, StringSplitOptions.RemoveEmptyEntries);

        // Список для хранения найденных существительных
        var nouns = new List<string>();

        // Проход по всем словам и добавление существительных в список
        foreach (var word in words)
        {
            if (word.Length >= 3)
            {
                var clearWord = word.RemoveNonAlphabeticCharacters();
                if (nounEndings.Any(ending => clearWord.EndsWith(ending, StringComparison.OrdinalIgnoreCase)))
                    nouns.Add(clearWord);
            }
        }

        return nouns.ToArray();
    }

    public static string RemoveFirstOccurrence(this string mainString, string toRemove)
    {
        if (mainString == null || toRemove == null)
        {
            throw new ArgumentNullException("Both strings must be non-null.");
        }

        int index = mainString.IndexOf(toRemove);
        if (index == -1)
        {
            // Если подстрока не найдена, возвращаем исходную строку
            return mainString;
        }

        // Удаляем первое вхождение подстроки
        return mainString.Remove(index, toRemove.Length);
    }


    public static string CapitalizeFirstLetter(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        return char.ToUpper(input[0]) + input.Substring(1);
    }
}