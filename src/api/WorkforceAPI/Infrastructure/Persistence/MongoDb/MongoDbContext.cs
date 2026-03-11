using MongoDB.Driver;
using WorkforceAPI.Domain.Entities;

namespace WorkforceAPI.Infrastructure.Persistence.MongoDb;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("Mongo")
            ?? throw new InvalidOperationException(
                "MongoDB connection string 'Mongo' is not configured.");

        var databaseName = configuration
            .GetConnectionString("MongoDatabaseName")
            ?? "WorkforceDb";

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);

        EnsureIndexes();
    }

    // ── Collections ───────────────────────────────────────────
    public IMongoCollection<LeaveRequest> LeaveRequests
        => _database.GetCollection<LeaveRequest>("LeaveRequests");

    public IMongoCollection<AuditLog> AuditLogs
        => _database.GetCollection<AuditLog>("AuditLogs");

    public IMongoCollection<DashboardReport> DashboardReports
        => _database.GetCollection<DashboardReport>("DashboardReports");

    // ── Indexes ───────────────────────────────────────────────
    private void EnsureIndexes()
    {
        // LeaveRequests
        LeaveRequests.Indexes.CreateOne(
            new CreateIndexModel<LeaveRequest>(
                Builders<LeaveRequest>.IndexKeys
                    .Ascending(x => x.EmployeeId)));

        LeaveRequests.Indexes.CreateOne(
            new CreateIndexModel<LeaveRequest>(
                Builders<LeaveRequest>.IndexKeys
                    .Ascending(x => x.Status)));

        // AuditLogs
        AuditLogs.Indexes.CreateOne(
            new CreateIndexModel<AuditLog>(
                Builders<AuditLog>.IndexKeys
                    .Ascending(x => x.AggregateType)
                    .Ascending(x => x.AggregateId)));

        AuditLogs.Indexes.CreateOne(
            new CreateIndexModel<AuditLog>(
                Builders<AuditLog>.IndexKeys
                    .Descending(x => x.OccurredAt)));

        // DashboardReports — unique on ReportKey
        DashboardReports.Indexes.CreateOne(
            new CreateIndexModel<DashboardReport>(
                Builders<DashboardReport>.IndexKeys
                    .Ascending(x => x.ReportKey),
                new CreateIndexOptions { Unique = true }));
    }
}