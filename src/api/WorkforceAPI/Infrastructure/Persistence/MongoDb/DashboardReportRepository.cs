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
        return await _collection
            .Find(x => x.ReportKey == "dashboard")
            .FirstOrDefaultAsync(ct);
    }

    public async Task UpsertAsync(
        DashboardReport report, CancellationToken ct = default)
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