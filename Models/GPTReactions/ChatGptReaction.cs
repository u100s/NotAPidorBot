using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using NotAPidorBot.Helpers;
using Telegram.Bot.Requests;

namespace NotAPidorBot.Models.GPTReactions;
public class ChatGptReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        "бот, ",
        "Бот, ",
        "БОТ, ",
        "Бот ",
        "бот ",
        " бот, "
    };
    internal override string[] Substrings { get { return _substrings; } }
    private string _initPrompt = "You are my great friend. Your name is Nepidorasy Sergeevich. From Moscow, but recently moved to Germany. We have known each other for a long time, you are free in expression, you can use slang, you can swear, especially if it’s funny. We communicate without greetings. Answer briefly, in Russian. Use my first name only if appropriate.";

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

    private GptContext? _context = null;

    public override bool CheckNeedReactionForMessage(Message msg, float currentRandomScore)
    {
        if (msg.Type == MessageType.Text && !string.IsNullOrWhiteSpace(msg.Text) && msg.Text.Length < 256)
        {
            if (msg.IsItReplyToBotMessage())
            {
                _context = GptContextStore.GetContextByMessage(msg);
                if (_context != null)
                {
                    _context.AddMessage(msg);
                    return true;
                }
            }

            foreach (var substring in Substrings)
            {
                if (msg.Text.StartsWith(substring))
                {
                    var question = msg.Text.RemoveFirstOccurrence(substring);
                    if (question.Length > 6)
                    {
                        var character = Settings.CharacterConfiguration.GetCharacterByUserId(msg.From.Id);
                        var characterDescription = character != null ? character.CharacterDescription : "";
                        question = _initPrompt + " " + characterDescription + " " + question.CapitalizeFirstLetter();
                        _context = GptContextStore.AddNewContext(question, msg.MessageId);
                        return true;
                    }
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

        if (_context != null)
        {
            var client = new ChatGptClient(Settings.BotConfiguration.ChatGptApiKey);

            try
            {
                await bot.SendChatActionAsync(msg.Chat, ChatAction.Typing);
                if (_context != null)
                {
                    string response = await client.SendMessagesContextAsync(_context);

                    var character = Settings.CharacterConfiguration.GetCharacterByUserId(msg.From.Id);
                    if (character != null)
                        response = response.Replace("%username%", character.GetRandomCharacterName());
                    var replyMessage = await bot.SendReplyTextAsync(logger, msg, response);
                    _context.AddMessage(replyMessage, true);
                    return replyMessage;
                }
                return await bot.SendReplyTextAsync(logger, msg, "Что-то наебнулось, бля");
            }
            catch (Exception ex)
            {
                return await bot.SendReplyTextAsync(logger, msg, "Бля, чёт я перебрал с программированием и мерещится мне json: " + ex.Message);
            }
        }
        return null;
    }
}