using JasperFx.Resources;
using Wolverine;
using Wolverine.Kafka;

public static class SetupWolverineKafkaExtensions
{
    public static void AddWolverineWithKafka(this ConfigureHostBuilder host)
    {
        host.UseWolverine(opts =>
        {
            opts.UseKafka(
                    Environment.GetEnvironmentVariable("services__kafka__kafka__0")
                        ?? ""
                )
                .AutoProvision();

            opts.PublishAllMessages().ToKafkaTopics();

            opts.ListenToKafkaTopic(nameof(EmailReceivedModel)).BufferedInMemory();

            opts.Policies.DisableConventionalLocalRouting();

            opts.Services.AddResourceSetupOnStartup();

            opts.Discovery.IncludeAssembly(typeof(EmailReceivedConsumer).Assembly);

            Console.WriteLine(
                opts.DescribeHandlerMatch(typeof(EmailReceivedConsumer))
            );
            Console.WriteLine("Wolverine with Kafka is configured.");
        });
    }
}
