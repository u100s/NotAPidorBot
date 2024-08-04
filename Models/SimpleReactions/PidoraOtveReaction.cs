using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models.SimpleReactions;
public class PidoraOtveReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        "не"
    };
    internal override string[] Substrings { get { return _substrings; } }

    private Reply[] _replies = new[]
    {
        new Reply("пидора отве")
    };
    internal override Reply[] Replies { get { return _replies; } }


    public override bool NeedFullMatch { get; internal set; } = true;
    public override int Probability { get; internal set; } = 100;
}