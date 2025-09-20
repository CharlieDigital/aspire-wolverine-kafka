// This is the Aspire AppHost that connects everything together.
// Start with: dotnet watch run --project dn-aspire.AppHost -lp http --non-interactive

var builder = DistributedApplication.CreateBuilder(args);

// Start Kafka as a container with the Kafka UI
var kafka = builder.AddKafka("kafka").WithKafkaUI();

var postgres = builder.AddPostgres("aspirepg").WithPgWeb().AddDatabase("webapipg");

// Start out .NET web app and connect Kafka to it.  Practically
// this sets the "ConnectionStrings__kafka" environment variable
// for the web application process started by Aspire
var api = builder
    .AddProject<Projects.webapi_brighter>("api")
    .WaitFor(kafka)
    .WaitFor(postgres)
    .WithReplicas(2)
    .WithReference(kafka)
    .WithReference(postgres);

builder.Build().Run();
