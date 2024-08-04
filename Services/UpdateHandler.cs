using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using NotAPidorBot.Helpers;
using NotAPidorBot.Models;
using System.Reflection;

namespace NotAPidorBot.Services;

public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger) : IUpdateHandler
{
    private static float MasterRandomScore = 1f;
    private static float ChangeSpeedStep = 0.5f;

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        logger.LogInformation("HandleError: {Exception}", exception);
        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(message),
            { EditedMessage: { } message } => OnEditedMessage(message),
            // { CallbackQuery: { } callbackQuery }            => OnCallbackQuery(callbackQuery),
            // { InlineQuery: { } inlineQuery }                => OnInlineQuery(inlineQuery),
            // { ChosenInlineResult: { } chosenInlineResult }  => OnChosenInlineResult(chosenInlineResult),
            // { Poll: { } poll }                              => OnPoll(poll),
            // { PollAnswer: { } pollAnswer }                  => OnPollAnswer(pollAnswer),
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            _ => UnknownUpdateHandlerAsync(update)
        });
    }

    private async Task OnMessage(Message msg)
    {
        logger.LogInformation("В чате '{ChatUsername}' {Username} пишет: {MessageText}", msg.Chat.Title, msg.From.Username, msg.Text != null ? msg.Text : msg.Type.ToString());

        if (Settings.BotConfiguration.AdminIds.Contains(msg.From.Id) && msg.Chat.Type == ChatType.Private)
        {
            await OnCreatorMessage(msg);
            return;
        }

        if (msg.Type != MessageType.Text)
        {
            return;
        }

        if (msg.Chat.Type == ChatType.Private)
        {
            await bot.SendRandomReplyTextAsync(logger, msg, "Добавь меня в групповой чат", "Я лучше в групповых чатах", "Чо ты в личку пишешь, давай групповуху", "дратути", "мискузи");
            return;
        }

        if (!Settings.BotConfiguration.ActiveChatIds.Contains(msg.Chat.Id))
        {
            await bot.SendRandomReplyTextAsync(logger, msg,
            "Вы кто такие? Я вас не звал. Идите нахуй!",
            "Я буду жаловаться!",
            "Продам собаку. Злая. Чавкает на чужих.",
            "Не раздевайтесь! Я уже ухожу.",
            "Бот за бесплатно не автоответчик.",
            "Некоторые гении, такие как я, считают ниже своего достоинства общаться с такими имбецилами как вы.",
            "Все люди бестолковые, но таких как тут - ни разу не видал."
            );
            return;
        }

        Message sentMessage = await CheckNeedSlowdownOrSpeedup(msg);
        if (sentMessage == null)
        {
            // Инициализируем массив подстрок и соответствующих методов
            // Заданное пространство имен
            string targetNamespace = "NotAPidorBot.Models";

            // Получаем все типы в текущей сборке
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();

            // Список для хранения экземпляров классов
            List<ReactionBase> reactions = new List<ReactionBase>();

            // Проходим по всем типам и ищем классы, наследующие ReactionBase
            foreach (Type type in types)
            {
                if (!String.IsNullOrEmpty(type.Namespace)
                    && type.Namespace.Contains(targetNamespace)
                    && type.IsClass
                    && !type.IsAbstract
                    && type.IsSubclassOf(typeof(ReactionBase)))
                {
                    // Создаем экземпляр класса и добавляем его в список
                    ReactionBase instance = (ReactionBase)Activator.CreateInstance(type);
                    reactions.Add(instance);
                }
            }

            // Работаем по реакциям
            foreach (var reaction in reactions)
            {
                if (reaction.CheckNeedReactionForMessage(msg, MasterRandomScore))
                {
                    sentMessage = await reaction.SendAsync(bot, logger, msg);
                    break; // Прекращаем поиск после первого найденного совпадения
                }
            }
        }

        if (sentMessage != null)
        {
            if (sentMessage.Type == MessageType.Text)
                logger.LogInformation("Я ответил: {MessageText}", sentMessage.Text);
            if (sentMessage.Type == MessageType.Sticker)
                logger.LogInformation("Я ответил: {MessageText}", sentMessage.Type);
        }
    }


    private async Task<Message> CheckNeedSlowdownOrSpeedup(Message msg)
    {
        Message sentMessage = null;
        bool speedChanged = false;

        if (msg.Type == MessageType.Text)
        {
            string message = msg.Text ?? "";

            if (msg.ReplyToMessage != null && msg.ReplyToMessage.From != null && msg.ReplyToMessage.From.IsBot && msg.ReplyToMessage.From.Username == Settings.BotConfiguration.BotUserName)
            {
                if (message.EqualsAny("притихни", "затихни", "полегче", "остынь", "успокойся", "притормози", "потише", "сбавь обороты", "тормози", "не горячись", "помедленнее", "спокойнее", "поаккуратнее", "притормозь", "не спеши", "попридержи коней", "остановись", "поуймись", "умерь пыл", "не торопись", "поосторожнее", "притихни-ка", "притуши", "хорош"))
                {
                    if (MasterRandomScore > 0)
                    {
                        MasterRandomScore = MasterRandomScore - ChangeSpeedStep;
                        speedChanged = true;
                    }
                    else
                        return await bot.SendRandomReplyTextAsync(logger, msg, "Я уже ниже травы, тише воды", "Давно молчу в трубочку не пикаю", "Я уже как мышь под веником", "Я уже и так в режиме стелс", "Тише меня только мертвые", "Я уже как привидение", "Я и так на минималках", "Я уже как статуя", "Я уже и так на мьюте", "Я тут как в вакууме", "Я уже и так беззвучный", "Я уже как сама тишина", "У меня уже рот на замке", "Я уже как тишина в библиотеке");
                }
                else if (message.EqualsAny("давай поболтаем", "говори", "молви хоть слово", "погромче", "громче", "не молчи", "выразись", "озвучь", "выскажись", "скажи что-нибудь", "поделись", "проговори", "расскажи", "выскажи своё мнение", "не стесняйся", "вырази свои мысли", "давай обсудим", "скажи вслух", "голосом", "озвучь свои мысли", "поговорим", "давай обсудим", "вырази вслух", "не бойся говорить", "вырази своё мнение"))
                {
                    if (MasterRandomScore < 1)
                    {
                        MasterRandomScore = MasterRandomScore + ChangeSpeedStep;
                        speedChanged = true;
                    }
                    else
                        return await bot.SendRandomReplyTextAsync(logger, msg, "Я на максималке", "Пиздоболю напропалую!", "Режим 'Язык без костей активирован!'", "Я уже на полную катушку", "Говорю, как могу!", "Максимальная громкость включена", "Я уже на пределе", "Выкладываю всё, что есть", "Я уже на всю мощь", "Я и так на полную", "Голосовые связки на максимум", "Я уже во всю глотку", "Я и так на всю катушку", "Уже на полную мощность", "Я уже как мегафон", "Говорю, что есть сил", "Я и так на пределе возможностей", "Я уже как радио", "Я уже на максимум", "Уже на всю", "Я и так на полную громкость", "Я уже как громкоговоритель", "Я уже на всю громкость", "Говорю, что есть мочи", "Я и так на максимум возможностей");
                }
                else if (message.EqualsAny("заткнись", "умолкни", "тихо", "замолчи", "прекрати", "стоп", "молчи", "перестань", "стопэ", "утихни", "шшш", "хватит", "закрой рот", "не говори", "молчать"))
                {
                    if (MasterRandomScore > 0)
                    {
                        MasterRandomScore = 0;
                        speedChanged = true;
                    }
                    else
                        return await bot.SendRandomReplyTextAsync(logger, msg, "Я уже ниже травы, тише воды", "Давно молчу в трубочку не пикаю", "Я уже как мышь под веником", "Я уже и так в режиме стелс", "Тише меня только мертвые", "Я уже как привидение", "Я и так на минималках", "Я уже как статуя", "Я уже и так на мьюте", "Я тут как в вакууме", "Я уже и так беззвучный", "Я уже как сама тишина", "У меня уже рот на замке", "Я уже как тишина в библиотеке");
                }
            }

            if (message.EqualsAny("бот живи", "бот говори") && MasterRandomScore == 0)
            {
                MasterRandomScore = 1;
                speedChanged = true;
            }
        }

        if (speedChanged)
        {
            if (MasterRandomScore == 0)
            {
                return await bot.SendRandomReplyTextAsync(logger, msg, "Заткнулся", "Умолк", "Молчу", "Замолкаю", "Больше ни слова", "Прекратил говорить", "Смолк", "Утих", "Больше не говорю", "Ушел в тишину", "Замолк", "Больше не произнесу ни слова", "Стих", "Прекратил болтать", "Закрыл рот", "Я абиделся о объявляю вам бойкот!", "Затих", "Больше не произнесу ни звука", "Слова больше не скажу", "Ушел в молчание", "Смолкну", "🤐", "Молчу, как рыба", "Слов больше не будет");
            }

            if (MasterRandomScore <= 0.5f)
            {
                return await bot.SendRandomReplyTextAsync(logger, msg, "Меньше стаффа, больше табака", "Буду болтать, но без хуйни", "Попробую быть адекватным", "Буду говорить, но сдержанно", "Буду общаться, но в меру", "Постараюсь без лишнего", "Буду говорить по делу", "Буду сдержаннее", "Постараюсь быть лаконичным", "Буду говорить, но без перебора", "Буду высказываться, но осторожно", "Буду говорить, но не слишком громко", "Буду говорить, но с умом", "Буду общаться, но спокойно", "Постараюсь не перебарщивать", "Буду говорить, но в рамках приличия", "Буду говорить, но без лишних эмоций", "Буду говорить, но без перебиваний", "Буду говорить, но без лишних слов", "Буду говорить, но не слишком много", "Буду говорить, но с уважением", "Буду говорить, но без лишних деталей", "Буду говорить, но кратко", "Буду говорить, но по существу", "Буду говорить, но без лишних комментариев", "Буду говорить, но сдержанно и по делу");
            }

            if (MasterRandomScore > 0.5f)
            {
                return await bot.SendRandomReplyTextAsync(logger, msg, "Больше стаффа, меньше табака", "Ща всё расскажу что думаю", "Ну теперь меня хер заткнёте", "Тебе интересно? Нет? Продолжаю говорить", "Теперь я всё выскажу", "Готовься, сейчас всё расскажу", "Теперь слушай внимательно", "Ща всё выложу", "Держись, сейчас будет поток мыслей", "Теперь меня не остановить", "Щас всё расскажу, как на духу", "Теперь будет монолог", "Сейчас всё выскажу", "Теперь я говорю без фильтра", "Ща всё выдам", "Готовься к откровениям", "Теперь слушай всё, что у меня на душе", "Теперь я не буду молчать", "Щас всё расскажу, что накипело", "Теперь я говорю всё как есть", "Щас всё выложу начистоту", "Теперь меня не заткнуть", "Щас всё расскажу, как есть", "Теперь слушай все мои мысли");
            }
        }

        return sentMessage;
    }










    private async Task OnEditedMessage(Message msg)
    {
        logger.LogInformation("В чате '{ChatUsername}' {Username} редактирует сообщение: {MessageText}", msg.Chat.Title, msg.From.Username, msg.Text);
    }


    private async Task OnCreatorMessage(Message msg)
    {
        switch (msg.Type)
        {
            case MessageType.Sticker:
                await bot.SendTextMessageAsync(msg.Chat, msg.Type.ToString() + ' ' + msg.Sticker.FileId, parseMode: ParseMode.Html, replyParameters: new ReplyParameters() { MessageId = msg.MessageId }, replyMarkup: new ReplyKeyboardRemove());
                break;
        }
    }





    async Task<Message> Usage(Message msg)
    {
        const string usage = "Привет, я Непидорбот.";
        return await bot.SendTextMessageAsync(msg.Chat, usage, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());
    }

    async Task<Message> SendPhoto(Message msg)
    {
        await bot.SendChatActionAsync(msg.Chat, ChatAction.UploadPhoto);
        await Task.Delay(2000); // simulate a long task`
        await using var fileStream = new FileStream("Files/bot.gif", FileMode.Open, FileAccess.Read);
        return await bot.SendPhotoAsync(msg.Chat, fileStream, caption: "Read https://telegrambots.github.io/book/");
    }

    // Send inline keyboard. You can process responses in OnCallbackQuery handler
    async Task<Message> SendInlineKeyboard(Message msg)
    {
        List<List<InlineKeyboardButton>> buttons =
        [
            ["1.1", "1.2", "1.3"],
            [
                InlineKeyboardButton.WithCallbackData("WithCallbackData", "CallbackData"),
                InlineKeyboardButton.WithUrl("WithUrl", "https://github.com/TelegramBots/Telegram.Bot")
            ],
        ];
        return await bot.SendTextMessageAsync(msg.Chat, "Inline buttons:", replyMarkup: new InlineKeyboardMarkup(buttons));
    }

    async Task<Message> SendReplyKeyboard(Message msg)
    {
        List<List<KeyboardButton>> keys =
        [
            ["1.1", "1.2", "1.3"],
            ["2.1", "2.2"],
        ];
        return await bot.SendTextMessageAsync(msg.Chat, "Keyboard buttons:", replyMarkup: new ReplyKeyboardMarkup(keys) { ResizeKeyboard = true });
    }

    async Task<Message> RemoveKeyboard(Message msg)
    {
        return await bot.SendTextMessageAsync(msg.Chat, "Removing keyboard", replyMarkup: new ReplyKeyboardRemove());
    }

    async Task<Message> RequestContactAndLocation(Message msg)
    {
        List<KeyboardButton> buttons =
            [
                KeyboardButton.WithRequestLocation("Location"),
                KeyboardButton.WithRequestContact("Contact"),
            ];
        return await bot.SendTextMessageAsync(msg.Chat, "Who or Where are you?", replyMarkup: new ReplyKeyboardMarkup(buttons));
    }

    async Task<Message> StartInlineQuery(Message msg)
    {
        var button = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode");
        return await bot.SendTextMessageAsync(msg.Chat, "Press the button to start Inline Query\n\n" +
            "(Make sure you enabled Inline Mode in @BotFather)", replyMarkup: new InlineKeyboardMarkup(button));
    }

    static Task<Message> FailingHandler(Message msg)
    {
        throw new NotImplementedException("FailingHandler");
    }

    // Process Inline Keyboard callback data
    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
        await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"Received {callbackQuery.Data}");
        await bot.SendTextMessageAsync(callbackQuery.Message!.Chat, $"Received {callbackQuery.Data}");
    }

    #region Inline Mode

    private async Task OnInlineQuery(InlineQuery inlineQuery)
    {
        logger.LogInformation("Received inline query from: {InlineQueryFromId}", inlineQuery.From.Id);

        InlineQueryResult[] results = [ // displayed result
            new InlineQueryResultArticle("1", "Telegram.Bot", new InputTextMessageContent("hello")),
            new InlineQueryResultArticle("2", "is the best", new InputTextMessageContent("world"))
        ];
        await bot.AnswerInlineQueryAsync(inlineQuery.Id, results, cacheTime: 0, isPersonal: true);
    }

    private async Task OnChosenInlineResult(ChosenInlineResult chosenInlineResult)
    {
        logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);
        await bot.SendTextMessageAsync(chosenInlineResult.From.Id, $"You chose result with Id: {chosenInlineResult.ResultId}");
    }

    #endregion

    private Task OnPoll(Poll poll)
    {
        logger.LogInformation("Received Poll info: {Question}", poll.Question);
        return Task.CompletedTask;
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}
