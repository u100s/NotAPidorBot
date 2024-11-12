using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NotAPidorBot.Models.TotalContext;
using NotAPidorBot.Stores;

namespace NotAPidorBot.Models.AdminCommands;
public class GetAllPersons : AdminCommandBase
{
    private string commandName = "/getallpersons";
    public override string CommandName { get { return commandName; } }

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