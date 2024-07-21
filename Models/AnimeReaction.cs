using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models;
public class AnimeReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        "аниме"
    };
    internal override string[] Substrings { get { return _substrings; } }

    private Reply[] _replies = new[]
    {
        new Reply("аниме-хуяниме"),
        new Reply("хуяниме")
    };
    internal override Reply[] Replies { get { return _replies; } }


    public override int Probability { get; internal set; } = 70;
}