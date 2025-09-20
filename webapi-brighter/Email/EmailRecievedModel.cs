using Paramore.Brighter;

[PublicationTopic("email.topic")]
public class EmailReceivedModel() : Event(Id.Random())
{
    public string To { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
