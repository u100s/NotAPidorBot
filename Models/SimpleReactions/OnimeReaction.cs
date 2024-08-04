using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models.SimpleReactions;
public class OnimeReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        "ониме",
        "онеме",
    };
    internal override string[] Substrings { get { return _substrings; } }

    private Reply[] _replies = new[]
    {
        new Reply("ониме-хуяниме"),
        new Reply("хуониме")
    };
    internal override Reply[] Replies { get { return _replies; } }


    public override int Probability { get; internal set; } = 90;
}