using System;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace NotAPidorBot.Helpers;

public static class BotHelpers
{
    public static async Task<Message> SendTextAsync(this ITelegramBotClient bot, ILogger logger, Chat chat, string text)
    {
        return await bot.SendTextMessageAsync(chat, text, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());
    }
    public static async Task<Message> SendReplyTextAsync(this ITelegramBotClient bot, ILogger logger, Message msg, string text)
    {
        return await bot.SendTextMessageAsync(msg.Chat, text, parseMode: ParseMode.Html, replyParameters: new ReplyParameters() { MessageId = msg.MessageId }, replyMarkup: new ReplyKeyboardRemove());
    }

    public static async Task<Message> SendReplyAsMarkdownTextAsync(this ITelegramBotClient bot, ILogger logger, Message msg, string text)
    {
        return await bot.SendTextMessageAsync(msg.Chat, text, parseMode: ParseMode.MarkdownV2, replyParameters: new ReplyParameters() { MessageId = msg.MessageId }, replyMarkup: new ReplyKeyboardRemove());
    }

    public static async Task<Message> SendReplyStickerAsync(this ITelegramBotClient bot, ILogger logger, Message msg, string stickerId)
    {
        return await bot.SendStickerAsync(msg.Chat, stickerId, replyParameters: new ReplyParameters() { MessageId = msg.MessageId }, replyMarkup: new ReplyKeyboardRemove());
    }

    public static async Task<Message> SendRandomReplyTextAsync(this ITelegramBotClient bot, ILogger logger, Message msg, params string[] replies)
    {
        // Проверка на пустой список
        if (replies == null || replies.Length == 0)
        {
            throw new ArgumentException("The list is empty or null.");
        }

        var random = new Random();
        int randomIndex = random.Next(replies.Length);

        var reply = replies[randomIndex];
        return await bot.SendReplyTextAsync(logger, msg, reply);
    }

    public static bool IsItReplyToBotMessage(this Message msg)
    {
        return msg != null &&
            msg.ReplyToMessage != null &&
            msg.ReplyToMessage.From != null &&
            msg.ReplyToMessage.From.IsBot &&
            msg.ReplyToMessage.From.Username == Settings.BotConfiguration.BotUserName;
    }
}