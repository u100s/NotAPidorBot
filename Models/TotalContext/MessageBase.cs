namespace NotAPidorBot.Models.TotalContext;
public abstract class MessageBase
{
    public string Text { get; private set; }

    public MessageBase(string text)
    {
        Text = text;
    }
}