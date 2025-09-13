var builder = WebApplication.CreateBuilder(args);

// Make logging single line
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
});

builder.AddServiceDefaults();

// Add other services to the container.
builder.Services.AddControllers();

// This is our custom method to setup Wolverine with Kafka
builder.Host.AddWolverineWithKafka();

var app = builder.Build();

app.MapControllers();

await app.RunAsync();
