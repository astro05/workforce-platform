using MongoDB.Driver;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Infrastructure.Persistence.MongoDb;

public class DashboardReportRepository : IDashboardReportRepository
{
    private readonly IMongoCollection<DashboardReport> _collection;

    public DashboardReportRepository(MongoDbContext ctx)
    {
        _collection = ctx.DashboardReports;
    }

    public async Task<DashboardReport?> GetLatestAsync(
        CancellationToken ct = default)
    {
        // Try both casing variants since report worker uses camelCase
        var filter = Builders<DashboardReport>.Filter.Or(
            Builders<DashboardReport>.Filter.Eq(x => x.ReportKey, "dashboard"),
            Builders<DashboardReport>.Filter.Eq("reportKey", "dashboard")
        );

        return await _collection
            .Find(filter)
            .FirstOrDefaultAsync(ct);
    }

    public async Task UpsertAsync(
        DashboardReport report,
        CancellationToken ct = default)
    {
        report.GeneratedAt = DateTime.UtcNow;
        report.ReportKey = "dashboard";

        await _collection.ReplaceOneAsync(
            x => x.ReportKey == "dashboard",
            report,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }
}