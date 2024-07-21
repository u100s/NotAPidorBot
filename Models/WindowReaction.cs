using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models;
public class WindowReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        "душно",
        "форточк",
        "душнила",
        "духота"
    };
    internal override string[] Substrings { get { return _substrings; } }

    private Reply[] _replies = new[]
    {
        new Reply("CAACAgIAAxkBAAMqZpFQogzjnR0edUwQR7_3ZXwntvwAAo8AA3yOWBX4b_8bJUsKszUE", MessageType.Sticker),
        new Reply("Открываем чат на проветривание")
    };
    internal override Reply[] Replies { get { return _replies; } }


    public override int Probability { get; internal set; } = 70;
}