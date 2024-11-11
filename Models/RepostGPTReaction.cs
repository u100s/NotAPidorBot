using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using NotAPidorBot.Helpers;
using NotAPidorBot.Characters;
using NotAPidorBot.Models.ChatGpt;
using NotAPidorBot.Models.TotalContext;

namespace NotAPidorBot.Models;
public class RepostGPTReaction : ReactionBase
{
    public override int Probability { get; internal set; } = 50;
    public override bool CheckNeedReactionForMessage(Telegram.Bot.Types.Message msg)
    {
        if (msg.ForwardFromChat != null && !string.IsNullOrWhiteSpace(msg.Text))
        {
            var random = new Random();
            return random.Next(0, 100) < Probability;
        }
        return false;
    }

    public override async Task<Telegram.Bot.Types.Message> SendAsync(ITelegramBotClient bot, ILogger logger, Telegram.Bot.Types.Message msg)
    {
        try
        {
            var message = GetTextFromMessage(msg);

            var client = new ChatGpt.Client(Settings.BotConfiguration.ChatGptApiKey);
            await bot.SendChatActionAsync(msg.Chat, ChatAction.Typing);
            string response = await client.SendMessagesContextAsync(CreateGptRequestBody(message));
            string replyText = CharacterHelper.DeanonimyzeText(response);
            var replyMessage = await bot.SendReplyTextAsync(logger, msg, replyText);
            Context.AddAnswerFromGpt(replyMessage.MessageId, response);
            return replyMessage;
        }
        catch (Exception ex)
        {
            return await bot.SendReplyTextAsync(logger, msg, "Бля, чёт я перебрал с программированием и мерещится мне: " + ex.Message + ", StackTrace: " + ex.StackTrace);
        }
    }


    private string GetTextFromMessage(Telegram.Bot.Types.Message msg)
    {
        if (msg.Type == Telegram.Bot.Types.Enums.MessageType.Text && !string.IsNullOrEmpty(msg.Text))
            return msg.Text;
        else if (!string.IsNullOrEmpty(msg.Caption))
            return "Изображение с подписью: " + msg.Caption;

        return "";
    }

    private static RequestBody CreateGptRequestBody(string message)
    {
        var messages = new List<ChatGpt.Message>();
        messages.Add(new ChatGpt.Message(message, false));
        messages.Add(new ChatGpt.Message("Придумай сарказмическую шутку по поводу этого сообщения", false));
        var result = new RequestBody
        {
            messages = messages.ToArray()
        };
        return result;
    }
}