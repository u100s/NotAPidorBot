using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models.SimpleReactions;
public class HuiReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        "хуй"
    };
    internal override string[] Substrings { get { return _substrings; } }

    private Reply[] _replies = new[]
    {
        new Reply("сам хуй"),
        new Reply("и похуй"),
        new Reply("от хуя слышу"),
    };
    internal override Reply[] Replies { get { return _replies; } }


    public override bool NeedFullMatch { get; internal set; } = true;
    public override int Probability { get; internal set; } = 100;
}