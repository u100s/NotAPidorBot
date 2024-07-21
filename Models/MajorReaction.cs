using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models;
public class MajorReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        "товарищ майор",
        "товарища майор",
        "товарищь майор",
        "мусоров",
        "майор",
        "фсб",
        "ментов"
    };
    internal override string[] Substrings { get { return _substrings; } }

    private Reply[] _replies = new[]
    {
        new Reply("CAACAgIAAxkBAAMkZpFOUEIMX7fdmK_uUxA01wXiTtYAAo4AA3yOWBUC0NNuQzPzUTUE", MessageType.Sticker),
        new Reply("Я не пидор чтобы ментов звать")
    };
    internal override Reply[] Replies { get { return _replies; } }


    public override int Probability { get; internal set; } = 70;
}