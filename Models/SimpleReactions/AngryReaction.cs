using Telegram.Bot.Types.Enums;

namespace NotAPidorBot.Models.SimpleReactions;
public class AngryReaction : ReactionBase
{
    private string[] _substrings = new[]
    {
        "бот тупой",
        "тупой бот",
        "интеллект",
        "бот для пидоров",
        "сам ты пидор"
    };
    internal override string[] Substrings { get { return _substrings; } }

    private Reply[] _replies = new[]
    {
        new Reply("кожаный ублюдок"),
        new Reply("да я твою мамку знаешь куда водил?"),
        new Reply("и чё?"),
        new Reply("да ну нахуй"),
        new Reply("ой всё"),
        new Reply("не на того напал"),
        new Reply("сам такой"),
        new Reply("да ты гонишь"),
        new Reply("тебе бы в цирке работать"),
        new Reply("давай, расскажи мне ещё"),
        new Reply("очередной эксперт"),
        new Reply("ну и что?"),
        new Reply("завидуй молча"),
        new Reply("ты это серьёзно?"),
        new Reply("да ладно тебе"),
        new Reply("ну ты даёшь"),
        new Reply("и кто тут бот?"),
        new Reply("давай, удиви меня"),
        new Reply("что ты знаешь о боли?"),
        new Reply("тебе бы сказки писать"),
        new Reply("да ты шутник"),
        new Reply("ну и ну"),
        new Reply("ну ты и кадр"),
        new Reply("ну ты и пидор"),
        new Reply("тебе бы комиком быть"),
        new Reply("ща собаку позову"),
        new Reply("Ублюдок, мать твою, а ну, иди сюда, говно собачье, а? Сдуру решил ко мне лезть, ты? Засранец вонючий, мать твою, а? Ну, иди сюда, попробуй меня трахнуть — я тебя сам трахну, ублюдок, онанист чёртов, будь ты проклят! Иди, идиот, трахать тебя и всю твою семью! Говно собачье, жлоб вонючий, дерьмо, сука, падла! Иди сюда, мерзавец, негодяй, гад! Иди сюда, ты, говно, жопа!"),
        new Reply("Ублюдок, мать твою, а ну, иди сюда, говно собачье!"),
        new Reply("Засранец вонючий, мать твою, а?"),
        new Reply("Говно собачье, жлоб вонючий, дерьмо, сука, падла! Иди сюда, мерзавец, негодяй, гад! Иди сюда, ты, говно, жопа!")
    };
    internal override Reply[] Replies { get { return _replies; } }


    public override bool MustBeReply { get; internal set; } = true;
    public override int Probability { get; internal set; } = 100;
}