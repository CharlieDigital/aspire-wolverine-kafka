using System.Text.Json;
using Wolverine.Attributes;

// Sample of a consumer.  Wolverine has a set of heuristics for discovery
[WolverineHandler]
public class EmailReceivedConsumer
{
    private readonly ILogger<EmailReceivedConsumer> _logger;

    public EmailReceivedConsumer(ILogger<EmailReceivedConsumer> logger)
    {
        _logger = logger;
    }

    public void Handle(EmailReceivedModel email)
    {
        _logger.LogInformation(
            "Received email: {Email}",
            JsonSerializer.Serialize(email)
        );
    }
}
