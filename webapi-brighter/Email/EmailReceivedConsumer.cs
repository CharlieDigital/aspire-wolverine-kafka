using System.Text.Json;
using Paramore.Brighter;

public class AnotherEmailReceivedConsumer(
    ILogger<AnotherEmailReceivedConsumer> logger
) : RequestHandlerAsync<EmailReceivedModel>
{
    public override async Task<EmailReceivedModel> HandleAsync(
        EmailReceivedModel email,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation(
            "<< RECEIVED EMAIL (2) >>: {Email}",
            JsonSerializer.Serialize(email)
        );
        return await base.HandleAsync(email, cancellationToken);
    }
}

public class EmailReceivedConsumer(ILogger<EmailReceivedConsumer> logger)
    : RequestHandlerAsync<EmailReceivedModel>
{
    public override async Task<EmailReceivedModel> HandleAsync(
        EmailReceivedModel email,
        CancellationToken cancellationToken = default
    )
    {
        // 👇 Mechanism to intentionally trigger error
        if (email.Subject.Contains("error"))
        {
            throw new Exception("Simulated exception");
        }

        logger.LogInformation(
            "<< RECEIVED EMAIL >>: {Email}",
            JsonSerializer.Serialize(email)
        );

        return await base.HandleAsync(email, cancellationToken);
    }
}
