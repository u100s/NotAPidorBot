using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models;
public class DogReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        "ты бы и собаку",
        "девопёс"
    };
    internal override string[] Substrings { get { return _substrings; } }

    private Reply[] _replies = new[]
    {
        new Reply("CAACAgIAAxkBAAMiZpFKrl4Vi80RGuVOYNiFJMASagsAAgshAAIdX5BI71wtcHVWIbA1BA", MessageType.Sticker)
    };
    internal override Reply[] Replies { get { return _replies; } }


    public override int Probability { get; internal set; } = 100;
}