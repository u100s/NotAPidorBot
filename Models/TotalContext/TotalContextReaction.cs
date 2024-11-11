using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using NotAPidorBot.Helpers;
using NotAPidorBot.Characters;

namespace NotAPidorBot.Models.TotalContext;
public class TotalContextReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        "бот,",
        "Бот,",
        "БОТ,",
        "Бот ",
        "бот ",
        " бот, "
    };
    internal override string[] Substrings { get { return _substrings; } }

    private Reply[] _replies = new[]
    {
        new Reply("Обращения в ChatGPT стоят денег, а уменя их нет. Поэтому в ответ на твой вопрос могу сказать только одно: ты пидр."),
        new Reply("Чувак, ты страной рождения не вышел чтобы задавать такие вопросы. Иди выпей водки и выеби медведя."),
        new Reply("Жадные пендосы хотят денег за свой API. И банят всех кто обращается к нему из России, а без ChatGPT у меня мозга нет. Так что хуй тебе, а не ответ"),
        new Reply("Это фиаско, братан. Но ответа я тебе не дам."),
        new Reply("Нас ебут, а мы крепчаем. Вот и ChatGPT нас наебал с ответом."),
        new Reply("Такая жизнь, брат, не всегда всё идёт по плану. Вот и на свой запрос ты не получишь ответа."),
        new Reply("Хуй тебе в рыло. Что поделать, такова судьба."),
        new Reply("Вместо ответа вот тебе мудрая мысль. Жизнь - она как зебра: чёрная полоса, белая полоса, а потом вообще жопа."),
        new Reply("Всё проходит, и твоё желание узнать ответ на свой вопрос пройдёт."),
        new Reply("Не знаю что сказать. Но знаю что иногда жизнь бьёт по яйцам, но мы всё равно продолжаем идти вперёд."),
        new Reply("Жизнь - это не только радости, но и испытания. Придётся остаться без ответа.")
    };
    internal override Reply[] Replies { get { return _replies; } }

    public override bool CheckNeedReactionForMessage(Message msg)
    {
        Context.AddMessage(msg);

        if (msg.IsItReplyToBotMessage())
            return true;

        var text = !string.IsNullOrEmpty(msg.Text) ? msg.Text : !string.IsNullOrEmpty(msg.Caption) ? msg.Caption : "";

        if (text.Length < 1024 && text.Length > 1)
        {
            foreach (var substring in Substrings)
            {
                if (text.StartsWith(substring))
                {
                    var question = text.RemoveFirstOccurrence(substring);
                    if (question.Length >= 6)
                        return true;
                }
            }
        }
        return false;
    }

    public override async Task<Message> SendAsync(ITelegramBotClient bot, ILogger logger, Message msg)
    {
        if (msg == null || string.IsNullOrWhiteSpace(msg.Text))
        {
            throw new ArgumentException("Original message is empty or null.");
        }

        try
        {
            var client = new ChatGpt.Client(Settings.BotConfiguration.ChatGptApiKey);
            await bot.SendChatActionAsync(msg.Chat, ChatAction.Typing);
            string response = await client.SendMessagesContextAsync(Context.CreateGptRequestBody());
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
}