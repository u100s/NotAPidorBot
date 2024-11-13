namespace NotAPidorBot.Models.TotalContext;
public class ChatMessage : MessageBase
{
    public int SpeakerId { get; private set; }
    public long MessageId { get; private set; }
    public bool IsAnswerFromGPT { get; private set; }
    public string ForwardedFrom { get; private set; }

    public DateTime Created { get; private set; }

    public ChatMessage(int speakerId, long messageId, string text, bool isAnswerFromGPT, string forwardedFrom = "") : base(text)
    {
        SpeakerId = speakerId;
        MessageId = messageId;
        IsAnswerFromGPT = isAnswerFromGPT;
        ForwardedFrom = forwardedFrom;
        Created = DateTime.Now;
    }
}