namespace NotAPidorBot.Models.TotalContext;
public class ChatMessage
{
    public int SpeakerId { get; private set; }
    public long MessageId { get; private set; }
    public string Text { get; private set; }
    public bool IsAnswerFromGPT { get; private set; }
    public string ForwardedFrom { get; private set; }

    public ChatMessage(int speakerId, long messageId, string text, bool isAnswerFromGPT, string forwardedFrom = "")
    {
        SpeakerId = speakerId;
        MessageId = messageId;
        Text = text;
        IsAnswerFromGPT = isAnswerFromGPT;
        ForwardedFrom = forwardedFrom;
    }
}