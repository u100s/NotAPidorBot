using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NotAPidorBot.Models.TotalContext;
using NotAPidorBot.Stores;

namespace NotAPidorBot.Models.AdminCommands;
public class GetAllPersons : AdminCommandBase
{
    public override bool CheckNeedReactionForMessage(Message msg)
    {
        if (msg.From != null && Settings.BotConfiguration.AdminIds.Contains(msg.From.Id) && msg.Chat.Type == ChatType.Private)
        {
            if (!string.IsNullOrWhiteSpace(msg.Text) && msg.Text.Trim().ToLower() == "/getallpersons")
                return true;
        }
        return false;
    }

    public override async Task<Message> SendAsync(ITelegramBotClient bot, ILogger logger, Message msg)
    {
        if (PersonsStore.Persons != null && PersonsStore.Persons.Count > 0)
        {
            foreach (var person in PersonsStore.Persons)
            {
                await bot.SendTextAsync(logger, msg.Chat, GetPersonInfo(person));
            }
        }
        else
            await bot.SendReplyTextAsync(logger, msg, "Список персон пуст");
        return null;
    }

    private string GetPersonInfo(Person person)
    {
        return string.Format("SpeakerId: {0}, TelegramUserId: {1}, Username: {2}, SpeakerName: {3}", person.SpeakerId, person.TelegramUserId, person.UserName, person.SpeakerName);
    }
}