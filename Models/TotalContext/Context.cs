using Telegram.Bot.Types;
using NotAPidorBot.Models.ChatGpt;
using System.Text.RegularExpressions;
using System;
using Microsoft.VisualBasic;
using System.Net.NetworkInformation;
using NotAPidorBot.Configurations;

namespace NotAPidorBot.Models.TotalContext;
public static class Context
{
    public static DateTime Created { get; private set; } = DateTime.Now;
    public static List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

    private static List<Person> Persons { get; set; } = new List<Person>();
    private static string RetellingText { get; set; } = "";


    public static void AddMessage(Telegram.Bot.Types.Message msg)
    {
        CheckAndCleanMessagesHistory();
        string messageText = GetTextFromAnyTypeMessage(msg);
        if (!string.IsNullOrWhiteSpace(messageText) && msg.From != null)
        {
            var person = GetOrCreatePersonByUser(msg.From.Id, msg.From.Username);
            var forwardedFromName = GetFromTitleForForwardedMessage(msg);
            Messages.Add(new ChatMessage(person.SpeakerId, msg.MessageId, messageText, false, forwardedFromName));
        }
    }

    public static RequestBody CreateGptRequestBody()
    {
        var messages = new List<ChatGpt.Message>();
        messages.Add(new ChatGpt.Message(AddCharactersDecriptionsToInitialMessage(), false));// Первое сообщение с инициализирующим промтом
        if (!string.IsNullOrWhiteSpace(RetellingText))
            messages.Add(new ChatGpt.Message("Ранее в этом чате обсуждали: " + RetellingText, false));// Пересказ того, что было раньше
        messages.AddRange(Messages.Select(m => new ChatGpt.Message(PrepareMessageTextToSendToGpt(m), m.IsAnswerFromGPT)));// Остальные сообщения из чата
        messages.Add(new ChatGpt.Message(Settings.CharacterConfiguration.LastMessageCondition, false));// Последнее сообщение, наставляющее на ответ
        var result = new RequestBody
        {
            messages = messages.ToArray()
        };
        return result;
    }

    public static void AddAnswerFromGpt(long messageId, string text)
    {
        CheckAndCleanMessagesHistory();
        if (!string.IsNullOrWhiteSpace(text))
        {
            Messages.Add(new ChatMessage(0, messageId, text, true));
        }
    }
    public static string ReplaceUserNamesByRealNames(string text)
    {
        string result = text;
        foreach (var p in Persons)
            result = result.Replace(p.SpeakerName, p.Character.GetRandomCharacterName());

        return result;
    }


    private static async void CheckAndCleanMessagesHistory()
    {
        if (Settings.ContextConfiguration.MessagesCountLimit <= Messages.Count)
        {
            var messagesToRetell = Messages.GetRange(0, Settings.ContextConfiguration.MessagesCountToRetell);
            var requestBody = CreateGptRequestBodyForRetelling(messagesToRetell);

            try
            {
                var client = new ChatGpt.Client(Settings.BotConfiguration.ChatGptApiKey);
                string response = await client.SendMessagesContextAsync(requestBody);
            }
            catch (Exception ex)
            {
                throw;
            }

            Messages.RemoveRange(0, Settings.ContextConfiguration.MessagesCountToRetell);
        }
    }

    private static RequestBody CreateGptRequestBodyForRetelling(List<ChatMessage> messagesToRetell)
    {
        var messages = new List<ChatGpt.Message>();
        messages.Add(new ChatGpt.Message(Settings.ContextConfiguration.RetellingInitPromtMessage, false));// Первое сообщение с инициализирующим промтом
        if (!string.IsNullOrWhiteSpace(RetellingText))
            messages.Add(new ChatGpt.Message("Ранее в этом чате обсуждали: " + RetellingText, false));// Пересказ того, что было раньше
        messages.AddRange(messagesToRetell.Select(m => new ChatGpt.Message(PrepareMessageTextToSendToGpt(m), m.IsAnswerFromGPT)));// Сообщения, которые пересказываем
        messages.Add(new ChatGpt.Message(Settings.ContextConfiguration.RetellingPostPromtMessage, false));// Последнее сообщение с коллтуэкшен для GPT
        var result = new RequestBody
        {
            messages = messages.ToArray()
        };
        return result;
    }

    private static string AddCharactersDecriptionsToInitialMessage()
    {
        var result = Settings.CharacterConfiguration.InitialMessage;
        var charactersDesciption = "В чате есть:";
        if (Persons.Count < 1)
            charactersDesciption += "ты один";
        else
        {
            foreach (var p in Persons)
                charactersDesciption += " " + p.SpeakerId.ToString() + ". " + p.IntroDescription;
        }

        // Добавляем текущее локальное время
        DateTime now = DateTime.Now;
        TimeZoneInfo localZone = TimeZoneInfo.Local;
        string formattedDate = string.Format("{0:dddd, MMMM dd, yyyy HH:mm:ss} {1}", now, localZone.DisplayName);

        charactersDesciption += " Now " + formattedDate;

        return result.Replace("%CharacterDescription%", charactersDesciption);
    }

    private static string GetTextFromAnyTypeMessage(Telegram.Bot.Types.Message msg)
    {
        if (msg.Type == Telegram.Bot.Types.Enums.MessageType.Text && !string.IsNullOrEmpty(msg.Text))
            return msg.Text;
        else if (!string.IsNullOrEmpty(msg.Caption))
            return msg.Caption;
        else
            return "";
    }

    private static string GetFromTitleForForwardedMessage(Telegram.Bot.Types.Message msg)
    {
        if (msg.ForwardFromChat != null)
        {
            if (!string.IsNullOrEmpty(msg.ForwardFromChat.Title))
                return msg.ForwardFromChat.Title;
            else if (!string.IsNullOrEmpty(msg.ForwardFromChat.Username))
                return msg.ForwardFromChat.Username;
        }
        else if (msg.ForwardFrom != null)
        {
            if (!string.IsNullOrEmpty(msg.ForwardFrom.Username))
                return msg.ForwardFrom.Username;
            else if (!string.IsNullOrEmpty(msg.ForwardFrom.LastName))
                return msg.ForwardFrom.LastName;
        }

        return "";
    }

    private static string PrepareMessageTextToSendToGpt(ChatMessage m)
    {
        string result = m.Text;
        // Меняем упоминания с тегами юзеров, типа @u100s на %username_1%, чтобы в ChatGPT отправлять меньше приватных данных
        foreach (var p in Persons)
            if (!string.IsNullOrWhiteSpace(p.Character.UserLogin))
            {
                result = Regex.Replace(result, "@" + p.Character.UserLogin, p.SpeakerName, RegexOptions.IgnoreCase);
            }

        // Добавляем в сообщение автора сообщения
        if (!m.IsAnswerFromGPT)
        {
            var messagePerson = FindPersonBySpeakerId(m.SpeakerId);
            string speakerName = messagePerson != null ? messagePerson.SpeakerName : "Кто-то";
            if (!string.IsNullOrEmpty(m.ForwardedFrom))
                result = messagePerson.SpeakerName + " переслал сообщение от " + m.ForwardedFrom + ": " + result;
            else
                result = messagePerson.SpeakerName + ": " + result;
        }
        return result;
    }

    private static Person? FindPersonBySpeakerId(int speakerId)
    {
        foreach (var p in Persons)
            if (p.SpeakerId == speakerId)
                return p;
        return null;
    }

    private static Person GetOrCreatePersonByUser(long userId, string? username)
    {
        Person? result = null;
        foreach (var p in Persons)
            if (p.TelegramUserId == userId)
                result = p;

        if (result == null)
        {
            var character = Settings.CharacterConfiguration.GetCharacterByUserId(userId);
            if (character == null)
                character = Character.CreateAnonimousCharacterByUserId(userId, username);
            result = new Person(Persons.Count + 1, userId, character);
            Persons.Add(result);
        }
        return result;
    }
}