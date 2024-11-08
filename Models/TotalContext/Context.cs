using Telegram.Bot.Types;
using NotAPidorBot.Models.ChatGpt;
using System;
using NotAPidorBot.Characters;
using System.Net.NetworkInformation;
using NotAPidorBot.Configurations;
using NotAPidorBot.Stores;

namespace NotAPidorBot.Models.TotalContext;
public static class Context
{
    public static DateTime Created { get; private set; } = DateTime.Now;
    public static List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

    public static string RetellingText { get; private set; } = "";


    public static void AddMessage(Telegram.Bot.Types.Message msg)
    {
        CheckAndCleanMessagesHistory();
        string messageText = GetTextFromAnyTypeMessage(msg);
        if (!string.IsNullOrWhiteSpace(messageText) && msg.From != null)
        {
            var username = !string.IsNullOrEmpty(msg.From.Username) ? msg.From.Username : string.Format("{0} {1}", msg.From.LastName, msg.From.FirstName);
            var person = PersonsStore.GetOrCreatePersonByUser(msg.From.Id, username);
            var forwardedFromName = GetFromTitleForForwardedMessage(msg);
            Messages.Add(new ChatMessage(person.SpeakerId, msg.MessageId, messageText, false, forwardedFromName));
        }
    }

    public static RequestBody CreateGptRequestBody()
    {
        var messages = new List<ChatGpt.Message>();
        messages.Add(new ChatGpt.Message(GetInitialMessage(), false));// Первое сообщение с инициализирующим промтом
        if (!string.IsNullOrWhiteSpace(RetellingText))
            messages.Add(new ChatGpt.Message("Ранее в этом чате обсуждали: " + RetellingText.AnonimyzeText(), false));// Пересказ того, что было раньше
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


    private static async void CheckAndCleanMessagesHistory()
    {
        if (Settings.ContextConfiguration.MessagesCountLimit <= Messages.Count)
        {
            var messagesToRetell = Messages.GetRange(0, Settings.ContextConfiguration.MessagesCountToRetell);
            var requestBody = CreateGptRequestBodyForRetelling(messagesToRetell);

            try
            {
                var client = new ChatGpt.Client(Settings.BotConfiguration.ChatGptApiKey);
                RetellingText = await client.SendMessagesContextAsync(requestBody);
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

    private static string GetInitialMessage()
    {
        // Берём текущее локальное время
        DateTime now = DateTime.Now;
        TimeZoneInfo localZone = TimeZoneInfo.Local;
        string formattedDate = string.Format("{0:dddd, MMMM dd, yyyy HH:mm:ss} {1}", now, localZone.DisplayName);

        // Берём описание всех участников чата
        string charactersDesciption = CharacterHelper.GetCharactersDecriptions() + " Now " + formattedDate;

        var result = Settings.CharacterConfiguration.InitialMessage;
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

    public static string PrepareMessageTextToSendToGpt(ChatMessage m)
    {
        string result = m.Text.AnonimyzeText();

        // Добавляем в сообщение автора сообщения
        if (!m.IsAnswerFromGPT)
        {
            var messagePerson = PersonsStore.FindPersonBySpeakerId(m.SpeakerId);
            string speakerName = messagePerson != null ? messagePerson.SpeakerName : "Кто-то";
            if (!string.IsNullOrEmpty(m.ForwardedFrom))
                result = speakerName + " переслал сообщение от " + m.ForwardedFrom + ": " + result;
            else
                result = speakerName + ": " + result;
        }
        return result;
    }
}