using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models.SimpleReactions;
public class MajorReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        "товарищ майор",
        "товарища майор",
        "товарищь майор",
        " мусоров",
        "майор",
        "фсб",
        " ментов"
    };
    internal override string[] Substrings { get { return _substrings; } }

    private Reply[] _replies = new[]
    {
        new Reply("CAACAgIAAxkBAAMkZpFOUEIMX7fdmK_uUxA01wXiTtYAAo4AA3yOWBUC0NNuQzPzUTUE", MessageType.Sticker),
        new Reply("CAACAgIAAxkBAAMCZq3pGaZrn5UsISdRs8M9bHhXzNYAAgEABGvpHrHLA5IfFNbbNQQ", MessageType.Sticker),
        new Reply("CAACAgIAAxkBAAMEZq3pHNDU0rjLdoqrCiFtblvvtzoAAgkABGvpHnKW1mseM6r2NQQ", MessageType.Sticker),
        new Reply("CAACAgIAAxkBAAMGZq3pJdtbkkwEV--Zg4V5zkEb3_IAAgYABGvpHnBQwAnwEcTcNQQ", MessageType.Sticker),
        new Reply("CAACAgIAAxkBAAMIZq3pKEv8F5M57DP3BKR6yiGW3KQAAgUABGvpHormrYLwSsDgNQQ", MessageType.Sticker),
        new Reply("CAACAgIAAxkBAAMKZq3pKmSm-Mxsxfj9Z7pzI52c120AAgQABGvpHofgNuJKyRjTNQQ", MessageType.Sticker),
        new Reply("CAACAgIAAxkBAAMMZq3pLqEBAqUccQPQAgFWyvch_UUAAgcABGvpHq9P4WDbf8H2NQQ", MessageType.Sticker),
        new Reply("CAACAgIAAxkBAAMOZq3pMYEicFvyqJBxoqqi4OMQs7EAAgoABGvpHn5eM5R9j2_UNQQ", MessageType.Sticker),
        new Reply("Я не пидор чтобы ментов звать"),
        new Reply("Не пидор я ментов чтобы звать")
    };
    internal override Reply[] Replies { get { return _replies; } }


    public override int Probability { get; internal set; } = 70;
}