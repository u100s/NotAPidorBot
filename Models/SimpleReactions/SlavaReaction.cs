using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models.SimpleReactions;
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
        new Reply("Славец удалец!"),  
        new Reply("Слава красава!"), 
        new Reply("Славик — наш герой"+Environment.NewLine+"С ним всегда мы за одно!"+Environment.NewLine+"Славик, ты вперед иди,"+Environment.NewLine+"Победу нам всем принеси!!")
    };
    internal override Reply[] Replies { get { return _replies; } }


    public override int Probability { get; internal set; } = 50;
}