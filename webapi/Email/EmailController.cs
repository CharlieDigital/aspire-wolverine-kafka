using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace webapi.Controllers;

[ApiController]
[Route("email")]
public class EmailController(ILogger<EmailController> logger, IMessageBus bus)
    : ControllerBase
{
    // Sample of a publisher
    [HttpPost("send")]
    public async Task Send()
    {
        var messageId = Random.Shared.Next(1000, 9999);

        logger.LogInformation("Sending test email...{MessageId}", messageId);

        var email = new EmailReceivedModel
        {
            To = "<recipient@example.com>",
            From = "<sender@example.com>",
            Subject = $"Test Email {messageId}",
            Body = "This is a test email."
        };

        await bus.PublishAsync(
            email,
            new DeliveryOptions { PartitionKey = email.To }
        );

        logger.LogInformation("Test email sent.");
    }
}
