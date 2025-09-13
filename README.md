# Aspire + Wolverine + Kafka

This repo demonstrates Wolverine + Kafka wired up through Aspire.

To run:

```shell
dotnet watch run --project dn-aspire.AppHost -lp http --non-interactive
```

Watch the console as a URL will be displayed for the dashboard.

Now you can `curl` the web API to publish a message:

```shell
curl -X POST http://localhost:5009/email/send -v
```

The first message takes a moment to arrive, but subsequent messages are instant.

This is a minimal configuration as a POC.

There are different patterns available for Kafka based on the type of messaging (partitions, etc.)

Not that this also includes a UI for Kafka (you can get the URL in the dashboard)
