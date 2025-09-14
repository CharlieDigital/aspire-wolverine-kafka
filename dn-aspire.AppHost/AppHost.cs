// This is the Aspire AppHost that connects everything together.
// Start with: dotnet watch run --project dn-aspire.AppHost -lp http --non-interactive

using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Start Kafka as a container with the Kafka UI
// var kafka = builder.AddKafka("kafka").WithKafkaUI();

// https://docs.warpstream.com/warpstream/getting-started/run-the-agent-locally
var kafka = builder
    .AddContainer("kafka", "public.ecr.aws/warpstream-labs/warpstream_agent:latest")
    .WithEndpoint(port: 9092, targetPort: 9092, "PLAINTEXT", "kafka")
    .WithEndpoint(port: 9094, targetPort: 9094, "http", "schema")
    // .WithEndpoint(port: 8080, targetPort: 8080, "http", "telemetry") // https://docs.warpstream.com/warpstream/agent-setup/monitor-the-warpstream-agents
    .WithArgs("playground");

// ❌ Can't get this to work with Warpstream
var kafkaui = builder
    .AddContainer("kafkaui", "provectuslabs/kafka-ui:latest")
    .WithEndpoint(port: 8080, targetPort: 8080, "http", "kafkaui")
    .WithEnvironment("DYNAMIC_CONFIG_ENABLED", "true")
    // .WithEnvironment("KAFKA_CLUSTERS_0_NAME", "kafka")
    // .WithEnvironment("KAFKA_CLUSTERS_0_BOOTSTRAPSERVERS", "kafka:9092")
    .WaitFor(kafka)
    .WithReference(kafka.GetEndpoint("kafka"));

// Start out .NET web app and connect Kafka to it.  Practically
// this sets the "ConnectionStrings__kafka" environment variable
// for the web application process started by Aspire
var api = builder
    .AddProject<Projects.webapi>("api1")
    .WaitFor(kafka)
    .WithReference(kafka.GetEndpoint("kafka"))
    .WithReplicas(2); // 👈 Two copies behind the proxy

builder.Build().Run();
