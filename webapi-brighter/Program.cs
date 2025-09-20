using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddBrighterWithKafka().AddControllers();

string connectionString =
    Environment.GetEnvironmentVariable("ConnectionStrings__webapipg") ?? "";

try
{
    await SetupPgAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Error setting up Postgres: {ex.Message}");
}

var app = builder.Build();

app.MapControllers();

app.Run();

async Task SetupPgAsync()
{
    await using NpgsqlConnection connection = new(connectionString);
    await connection.OpenAsync();
    await using var inboxCommand = connection.CreateCommand();
    inboxCommand.CommandText = """
        CREATE TABLE IF NOT EXISTS "inboxmessages"
        (
          CommandId VARCHAR(256) NOT NULL ,
          CommandType VARCHAR(256) NULL ,
          CommandBody TEXT NULL ,
          Timestamp timestamptz  NULL ,
          ContextKey VARCHAR(256) NULL,
          PRIMARY KEY (CommandId, ContextKey)
        );
        """;

    _ = await inboxCommand.ExecuteNonQueryAsync();

    await using var outboxCommand = connection.CreateCommand();
    outboxCommand.CommandText = """
        CREATE TABLE IF NOT EXISTS "outboxmessages"
        (
          Id bigserial PRIMARY KEY,
          MessageId character varying(255) UNIQUE NOT NULL,
          Topic character varying(255) NULL,
          MessageType character varying(32) NULL,
          Timestamp timestamptz NULL,
          CorrelationId character varying(255) NULL,
          ReplyTo character varying(255) NULL,
          ContentType character varying(128) NULL,
          PartitionKey character varying(128) NULL,
          WorkflowId character varying(255) NULL,
          JobId character varying(255) NULL,
          Dispatched timestamptz NULL,
          HeaderBag text NULL,
          Body text NULL,
          Source character varying (255) NULL,
          Type character varying (255) NULL,
          DataSchema character varying (255) NULL,
          Subject character varying (255) NULL,
          TraceParent character varying (255) NULL,
          TraceState character varying (255) NULL,
          Baggage text NULL
        );
        """;

    _ = await outboxCommand.ExecuteNonQueryAsync();
}
