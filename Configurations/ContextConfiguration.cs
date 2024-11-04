namespace NotAPidorBot.Configurations;
public class ContextConfiguration
{
    public string RetellingInitPromtMessage { get; set; } = "";
    public string RetellingPostPromtMessage { get; set; } = "";
    public int MessagesCountLimit { get; set; }
    public int MessagesCountToRetell { get; set; }
}
