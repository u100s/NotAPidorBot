using NotAPidorBot.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NotAPidorBot.Models.TotalContext;

namespace NotAPidorBot.Models.AdminCommands;
public class ChangeLastCondition : AdminCommandBase
{
    private string commandName = "/changecondition";
    public override string CommandName { get { return commandName; } }

    public override async Task<Message> SendAsync(ITelegramBotClient bot, ILogger logger, Message msg)
    {
        if (!string.IsNullOrWhiteSpace(msg.Text))
        {
            string condition = msg.Text.Replace(commandName, "").Trim();

            if (!string.IsNullOrWhiteSpace(condition))
            {
                Settings.CharacterConfiguration.LastMessageCondition = condition;
                await bot.SendReplyTextAsync(logger, msg, "Последнее условие заменено на это: " + Settings.CharacterConfiguration.LastMessageCondition);
            }
            else
                await bot.SendReplyTextAsync(logger, msg, "Новое условие не передано, оставлено прежнее: " + Settings.CharacterConfiguration.LastMessageCondition);
        }
        return null;
    }
}