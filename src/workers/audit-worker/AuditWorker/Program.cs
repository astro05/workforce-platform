using AuditWorker.Consumers;
using AuditWorker.Handlers;
using AuditWorker.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting Audit Worker");

    var builder = Host.CreateApplicationBuilder(args);

    // ── Serilog ───────────────────────────────────────────────
    builder.Services.AddSerilog();

    // ── MongoDB ───────────────────────────────────────────────
    builder.Services.AddSingleton<AuditMongoContext>();

    // ── RabbitMQ Consumer ─────────────────────────────────────
    builder.Services.AddHostedService<RabbitMqConsumer>();

    // ── Health Checks ─────────────────────────────────────────
    builder.Services.AddHealthChecks()
        .AddCheck<AuditWorkerHealthCheck>("mongodb");

    // ── HTTP for health endpoint ──────────────────────────────
    builder.Services.AddSingleton<IHealthCheck, AuditWorkerHealthCheck>();

    var host = builder.Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Audit Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}