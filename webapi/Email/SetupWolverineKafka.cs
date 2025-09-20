using JasperFx.Resources;
using Wolverine;
using Wolverine.Kafka;
using Wolverine.Postgresql;

public static class SetupWolverineKafkaExtensions
{
    public static void AddWolverineWithKafka(this ConfigureHostBuilder host)
    {
        // host.UseWolverine(opts =>
        // {
        //     opts.Discovery.IncludeAssembly(typeof(EmailReceivedConsumer).Assembly);

        //     Console.WriteLine(
        //         opts.DescribeHandlerMatch(typeof(EmailReceivedConsumer))
        //     );

        //     Console.WriteLine(
        //         opts.DescribeHandlerMatch(typeof(AnotherEmailReceivedConsumer))
        //     );

        //     Console.WriteLine("Wolverine with Kafka is configured.");
        // });

        host.UseWolverine(opts =>
        {
            opts.UseKafka(
                    Environment.GetEnvironmentVariable("ConnectionStrings__kafka")
                        ?? ""
                )
                .AutoProvision();

            opts.PersistMessagesWithPostgresql(
                Environment.GetEnvironmentVariable("ConnectionStrings__webapipg")
                    ?? ""
            );

            opts.PublishAllMessages().ToKafkaTopics();

            // Two named listeners
            opts.ListenToKafkaTopic(nameof(EmailReceivedModel));

            opts.Policies.UseDurableInboxOnAllListeners();
            // opts.Policies.DisableConventionalLocalRouting();

            opts.Durability.MessageStorageSchemaName = "wolverine";
            // Important in modular monoliths so that handlers are not grouped
            // together in one call.
            opts.MultipleHandlerBehavior = MultipleHandlerBehavior.Separated;

            // Treat each message as separate for different handlers in same process.
            opts.Durability.MessageIdentity = MessageIdentity.IdAndDestination;

            opts.Services.AddResourceSetupOnStartup();

            opts.Discovery.IncludeAssembly(typeof(EmailReceivedConsumer).Assembly);

            Console.WriteLine(
                opts.DescribeHandlerMatch(typeof(EmailReceivedConsumer))
            );

            Console.WriteLine(
                opts.DescribeHandlerMatch(typeof(AnotherEmailReceivedConsumer))
            );

            Console.WriteLine("Wolverine with Kafka is configured.");
        });

        // For Postgres database provisioning
        host.UseResourceSetupOnStartup();
    }
}
