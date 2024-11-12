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
using NotAPidorBot.Models.AdminCommands;

namespace NotAPidorBot.Services;

public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger) : IUpdateHandler
{
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
            await OnAdminMessage(msg);
            return;
        }

        if (msg.Type != MessageType.Text && msg.Type != MessageType.Photo && msg.Type != MessageType.Video)
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


        // Список реакций
        List<ReactionBase> reactions = ReactionHelper.GetReactionsList("NotAPidorBot.Models");

        // Работаем по реакциям
        foreach (var reaction in reactions)
        {
            if (reaction.CheckNeedReactionForMessage(msg))
            {
                Message sentMessage = await reaction.SendAsync(bot, logger, msg);

                if (sentMessage != null)
                {
                    if (sentMessage.Type == MessageType.Text)
                        logger.LogInformation("Я ответил: {MessageText}", sentMessage.Text);
                    if (sentMessage.Type == MessageType.Sticker)
                        logger.LogInformation("Я ответил: {MessageText}", sentMessage.Type);
                }
                break; // Прекращаем поиск после первого найденного совпадения
            }
        }
    }

    private async Task OnEditedMessage(Message msg)
    {
        logger.LogInformation("В чате '{ChatUsername}' {Username} редактирует сообщение: {MessageText}", msg.Chat.Title, msg.From.Username, msg.Text);
    }

    private async Task OnAdminMessage(Message msg)
    {
        var commands = ReactionHelper.GetReactionsList("NotAPidorBot.Models.AdminCommands");

        // Работаем по реакциям
        foreach (var command in commands)
        {
            if (command.CheckNeedReactionForMessage(msg))
            {
                Message sentMessage = await command.SendAsync(bot, logger, msg);
                break; // Прекращаем поиск после первого найденного совпадения
            }
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
