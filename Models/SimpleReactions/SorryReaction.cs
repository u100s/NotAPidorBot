using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models.SimpleReactions;
public class SorryReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        "бот извинись",
        "извинись бот",
        "бот заебал",
        "бот надоел"
    };
    internal override string[] Substrings { get { return _substrings; } }

    private Reply[] _replies = new[]
    {
        new Reply("сорри"),
        new Reply("прости"),
        new Reply("извините"),
        new Reply("прошу прощения"),
        new Reply("скузян"),
        new Reply("мискузи"),
        new Reply("мои извинения"),
        new Reply("виноват"),
        new Reply("прошу извинить"),
        new Reply("прошу прощенья"),
        new Reply("извиняюсь"),
        new Reply("пардон"),
        new Reply("простите"),
        new Reply("извиняюсь за это"),
        new Reply("виноват, исправлюсь"),
        new Reply("приношу извинения"),
        new Reply("простите меня"),
        new Reply("прошу прощения за это"),
        new Reply("извините, пожалуйста"),
        new Reply("прощенья прошу"),
        new Reply("извините, виноват"),
        new Reply("извините, моя ошибка"),
        new Reply("извините, не хотел"),

        new Reply("пацаны не извиняются"),
        new Reply("Да пошёл ты нахуй, извиняться ещё. Было бы перед кем!")
    };
    internal override Reply[] Replies { get { return _replies; } }


    public override int Probability { get; internal set; } = 90;
}