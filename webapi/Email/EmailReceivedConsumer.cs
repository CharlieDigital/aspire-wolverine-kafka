using System.Text.Json;
using Wolverine.Attributes;

// Sample of a consumer.  Wolverine has a set of heuristics for discovery
[WolverineHandler]
public class AnotherEmailReceivedConsumer(
    ILogger<AnotherEmailReceivedConsumer> logger
)
{
    public void Handle(EmailReceivedModel email)
    {
        logger.LogInformation(
            "<< RECEIVED EMAIL (2) >>: {Email}",
            JsonSerializer.Serialize(email)
        );
    }
}

// Sample of a consumer.  Wolverine has a set of heuristics for discovery
[WolverineHandler]
public class EmailReceivedConsumer(ILogger<EmailReceivedConsumer> logger)
{
    [RequeueOn(typeof(Exception), 2)]
    public void Handle(EmailReceivedModel email)
    {
        if (email.Subject.Contains("error"))
        {
            throw new Exception("Simulated exception");
        }

        logger.LogInformation(
            "<< RECEIVED EMAIL >>: {Email}",
            JsonSerializer.Serialize(email)
        );
    }
}
