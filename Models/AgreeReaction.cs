using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models;
public class AgreeReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        " для пидоров",
        " для пидорасов",
        " для педиков",
        " для геев"
    };
    internal override string[] Substrings { get { return _substrings; } }

    private Reply[] _replies = new[]
    {
        new Reply("Похоже на то"),
        new Reply("+"),
        new Reply("+1"),
        new Reply("Однозначно"),
        new Reply("Согл"),
        new Reply("100%"),
        new Reply("Бля буду"),
        new Reply("Точно"),
        new Reply("Абсолютно"),
        new Reply("Полностью согласен"),
        new Reply("Верно"),
        new Reply("Так и есть"),
        new Reply("Естественно"),
        new Reply("Безусловно"),
        new Reply("Несомненно"),
        new Reply("В точку"),
        new Reply("Как раз"),
        new Reply("И то верно"),
        new Reply("Поддерживаю"),
        new Reply("Вполне"),
        new Reply("Конечно"),
        new Reply("Разумеется"),
        new Reply("Ясное дело"),
        new Reply("Само собой"),
        new Reply("Плюсую"),
        new Reply("Плюсик"),
        new Reply("Истинно")
    };
    internal override Reply[] Replies { get { return _replies; } }


    public override int Probability { get; internal set; } = 90;
}