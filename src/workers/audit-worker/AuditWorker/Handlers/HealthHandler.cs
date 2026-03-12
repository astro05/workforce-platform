using AuditWorker.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AuditWorker.Handlers;

public class AuditWorkerHealthCheck : IHealthCheck
{
    private readonly AuditMongoContext _mongo;

    public AuditWorkerHealthCheck(AuditMongoContext mongo)
    {
        _mongo = mongo;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken ct = default)
    {
        try
        {
            // Ping MongoDB
            await _mongo.AuditLogs.Database
                .RunCommandAsync<MongoDB.Bson.BsonDocument>(
                    new MongoDB.Bson.BsonDocument("ping", 1),
                    cancellationToken: ct);

            return HealthCheckResult.Healthy(
                "Audit Worker is healthy — MongoDB connected");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Audit Worker is unhealthy — MongoDB not reachable",
                ex);
        }
    }
}