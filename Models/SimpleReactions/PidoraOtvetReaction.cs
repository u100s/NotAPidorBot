using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models.SimpleReactions;
public class PidoraOtvetReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        "нет"
    };
    internal override string[] Substrings { get { return _substrings; } }

    private Reply[] _replies = new[]
    {
        new Reply("пидора ответ")
    };
    internal override Reply[] Replies { get { return _replies; } }


    public override bool NeedFullMatch { get; internal set; } = true;
    public override int Probability { get; internal set; } = 100;
}