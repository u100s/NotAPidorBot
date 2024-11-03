using Telegram.Bot.Types;
using NotAPidorBot.Models.ChatGpt;
using System.Text.RegularExpressions;
using System;
using Microsoft.VisualBasic;
using System.Net.NetworkInformation;

namespace NotAPidorBot.Models.TotalContext;
public static class Context
{
    public static DateTime Created { get; private set; } = DateTime.Now;
    private static List<Person> Persons { get; set; } = new List<Person>();
    public static List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    private static int TotalMessagesLength { get; set; } = 0;
    private static int TotalMessagesLengthLimit { get; set; } = 30000;
    private static int LastMessagesCountForAnswer { get; set; } = 30;

    public static void AddMessage(Telegram.Bot.Types.Message msg)
    {
        CheckAndCleanMessageshistory();
        string messageText = GetTextFromAnyTypeMessage(msg);
        if (!string.IsNullOrWhiteSpace(messageText) && msg.From != null)
        {
            var preson = GetOrCreatePersonByUser(msg.From);
            var forwardedFromName = GetFromTitleForForwardedMessage(msg);
            Messages.Add(new ChatMessage(preson.SpeakerId, msg.MessageId, messageText, false, forwardedFromName));
        }
    }

    public static void AddAnswerFromGpt(long messageId, string text)
    {
        CheckAndCleanMessageshistory();
        if (!string.IsNullOrWhiteSpace(text))
        {
            Messages.Add(new ChatMessage(0, messageId, text, true));
            TotalMessagesLength += text.Length;
        }
    }

    public static string ReplaceUserNamesByRealNames(string text)
    {
        string result = text;
        foreach (var p in Persons)
            result = result.Replace(p.SpeakerName, p.Character.GetRandomCharacterName());

        return result;
    }

    private static void CheckAndCleanMessageshistory()
    {
        if (TotalMessagesLengthLimit < TotalMessagesLength)
        {
            while (TotalMessagesLengthLimit < TotalMessagesLength)
            {
                int removedLength = Messages[0].Text.Length;
                Messages.RemoveAt(0);
                TotalMessagesLength -= removedLength;
            }
        }
    }

    private static Person GetOrCreatePersonByUser(User user)
    {
        Person? result = null;
        foreach (var p in Persons)
            if (p.TelegramUserId == user.Id)
                result = p;

        if (result == null)
        {
            var character = Settings.CharacterConfiguration.GetCharacterByUserId(user.Id);
            if (character == null)
                character = Character.CreateAnonimousCharacterByUserId(user.Id, user.Username);
            result = new Person(Persons.Count + 1, user.Id, character);
            Persons.Add(result);
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

    public static RequestBody CreateGptRequestBody()
    {
        var messages = new List<ChatGpt.Message>();
        messages.Add(new ChatGpt.Message(AddCharactersDecriptionsToInitialMessage(), false));// Первое сообщение с инициализирующим промтом
        messages.AddRange(Messages.TakeLast(LastMessagesCountForAnswer).Select(m => new ChatGpt.Message(PrepareMessageTextToSendToGpt(m), m.IsAnswerFromGPT)));// Остальные сообщения из чата
        messages.Add(new ChatGpt.Message(Settings.CharacterConfiguration.LastMessageCondition, false));// Последнее сообщение, наставляющее на ответ
        var result = new RequestBody
        {
            messages = messages.ToArray()
        };
        return result;
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
                result = messagePerson.SpeakerName + " пишет: " + result;
        }
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
        charactersDesciption += " Now " + DateTime.Now.Date.ToLongDateString() + " " + DateTime.Now.ToShortTimeString();

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
}