using System.Data.Common;
using System.Reflection;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.Inbox.Postgres;
using Paramore.Brighter.MessageMappers;
using Paramore.Brighter.MessagingGateway.Kafka;
using Paramore.Brighter.Outbox.Hosting;
using Paramore.Brighter.Outbox.PostgreSql;
using Paramore.Brighter.PostgreSql;
using Paramore.Brighter.ServiceActivator.Extensions.DependencyInjection;
using Paramore.Brighter.ServiceActivator.Extensions.Hosting;

public static class SetupBrighterKafkaExtensions
{
    private static string connectionString =>
        Environment.GetEnvironmentVariable("ConnectionStrings__webapipg") ?? "";

    public static IServiceCollection AddBrighterWithKafka(
        this IServiceCollection services
    )
    {
        var kafka = new KafkaProducerRegistryFactory(
            new KafkaMessagingGatewayConfiguration
            {
                Name = "brighter-kafka",
                BootStrapServers =
                [
                    Environment.GetEnvironmentVariable("ConnectionStrings__kafka")
                        ?? ""
                ],
                SecurityProtocol = SecurityProtocol.Plaintext
            },
            [
                // TODO: Create one for each publish topic.
                new KafkaPublication<EmailReceivedModel>
                {
                    MakeChannels = OnMissingChannel.Create,
                    Source = new Uri("aspire-kafka", UriKind.RelativeOrAbsolute),
                    Topic = new RoutingKey("email.topic")
                }
            ]
        ).Create();

        var db = new RelationalDatabaseConfiguration(
            connectionString,
            "brightertests",
            outBoxTableName: "outboxmessages",
            inboxTableName: "inboxmessages"
        );

        var outbox = new PostgreSqlOutbox(db);
        var inbox = new PostgreSqlInbox(db);

        services
            .AddHostedService<ServiceActivatorHostedService>()
            .AddSingleton<IAmARelationalDatabaseConfiguration>(db)
            .AddSingleton<IAmAnOutbox>(outbox)
            .AddConsumers(options =>
            {
                // options.InboxConfiguration = new InboxConfiguration(inbox);

                options.Subscriptions =
                [
                    // TODO: Auto generate one for each consumer
                    new KafkaSubscription<EmailReceivedModel>(
                        new SubscriptionName("email.topic.sub"),
                        new ChannelName("email.topic"),
                        new RoutingKey("email.topic"),
                        groupId: "email-consumer",
                        makeChannels: OnMissingChannel.Create,
                        messagePumpType: MessagePumpType.Reactor
                    )
                ];
            })
            .AutoFromAssemblies([Assembly.GetExecutingAssembly()])
            .MapperRegistry(registry =>
                registry.SetDefaultMessageMapper(
                    typeof(CloudEventJsonMessageMapper<>)
                )
            )
            .AddProducers(options =>
            {
                options.ConnectionProvider = typeof(PostgreSqlTransactionProvider);
                options.TransactionProvider = typeof(PostgreSqlTransactionProvider);
                options.Outbox = outbox;
                options.ProducerRegistry = kafka;
            })
            .UseOutboxSweeper(options =>
            {
                options.BatchSize = 10;
            })
            .UseOutboxArchiver<DbTransaction>(
                new NullOutboxArchiveProvider(),
                opt => opt.MinimumAge = TimeSpan.FromMinutes(1)
            );

        return services;
    }
}
