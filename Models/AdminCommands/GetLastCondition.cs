using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NotAPidorBot.Models.TotalContext;

namespace NotAPidorBot.Models.AdminCommands;
public class GetLastCondition : AdminCommandBase
{
    private string commandName = "/getcondition";
    public override string CommandName { get { return commandName; } }

    public override async Task<Message> SendAsync(ITelegramBotClient bot, ILogger logger, Message msg)
    {
        await bot.SendReplyTextAsync(logger, msg, Settings.CharacterConfiguration.LastMessageCondition);

        return null;
    }
}