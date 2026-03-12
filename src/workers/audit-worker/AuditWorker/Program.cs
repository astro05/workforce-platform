using AuditWorker.Consumers;
using AuditWorker.Handlers;
using AuditWorker.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] [AUDIT-WORKER] {Message:lj}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Audit Worker");

    var builder = Host.CreateApplicationBuilder(args);

    // ── Serilog ───────────────────────────────────────────────
    builder.Services.AddSerilog();

    // ── MongoDB ───────────────────────────────────────────────
    builder.Services.AddSingleton<AuditMongoContext>();

    // ── Health Check ──────────────────────────────────────────
    builder.Services.AddSingleton<AuditWorkerHealthCheck>();

    // ── RabbitMQ Consumer ─────────────────────────────────────
    builder.Services.AddHostedService<RabbitMqConsumer>();

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