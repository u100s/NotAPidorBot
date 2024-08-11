
namespace NotAPidorBot.Models.ChatGpt;
public class Response
{
    public string id { get; set; }
    public string model { get; set; }
    public string created { get; set; }
    public Choice[] choices { get; set; }

    public class Choice
    {
        public int index { get; set; }
        public Message message { get; set; }

        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }
    }
}
