using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models;
public class SlavaReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        "Славу",
        "Слава",
        "Славе",
        "Славик",
        "Славон"
    };
    internal override string[] Substrings { get { return _substrings; } }

    private Reply[] _replies = new[]
    {
        new Reply("Слава Славе!"),
        new Reply("Слався Слава!"),
        new Reply("Слово Славе"),
        new Reply("Славчик красавчик!"),
        new Reply("Славец кросавец!"),
        new Reply("Здрав будь боярин!")
    };
    internal override Reply[] Replies { get { return _replies; } }


    public override int Probability { get; internal set; } = 50;
}