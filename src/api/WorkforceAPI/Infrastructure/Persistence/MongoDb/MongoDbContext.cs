using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using WorkforceAPI.Domain.Entities;

namespace WorkforceAPI.Infrastructure.Persistence.MongoDb;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    // ── Static constructor — runs once on first use ────────────
    static MongoDbContext()
    {
        // Register camelCase convention globally so .NET can
        // read documents written by the Node.js report worker
        // which serializes field names in camelCase by default
        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention()
        };

        ConventionRegistry.Register(
            "CamelCase",
            pack,
            _ => true);
    }

    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("Mongo")
            ?? throw new InvalidOperationException(
                "MongoDB connection string 'Mongo' " +
                "is not configured.");

        var databaseName = configuration
            .GetConnectionString("MongoDatabaseName")
            ?? "WorkforceDb";

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);

        EnsureIndexes();
    }

    // ── Collections ───────────────────────────────────────────
    public IMongoCollection<LeaveRequest> LeaveRequests
        => _database.GetCollection<LeaveRequest>(
            "LeaveRequests");

    public IMongoCollection<AuditLog> AuditLogs
        => _database.GetCollection<AuditLog>("AuditLogs");

    public IMongoCollection<DashboardReport> DashboardReports
        => _database.GetCollection<DashboardReport>(
            "DashboardReports");

    // ── Indexes ───────────────────────────────────────────────
    private void EnsureIndexes()
    {
        // LeaveRequests — index on EmployeeId and Status
        LeaveRequests.Indexes.CreateOne(
            new CreateIndexModel<LeaveRequest>(
                Builders<LeaveRequest>.IndexKeys
                    .Ascending(x => x.EmployeeId)));

        LeaveRequests.Indexes.CreateOne(
            new CreateIndexModel<LeaveRequest>(
                Builders<LeaveRequest>.IndexKeys
                    .Ascending(x => x.Status)));

        // AuditLogs — index on AggregateType+AggregateId
        // and OccurredAt for efficient querying
        AuditLogs.Indexes.CreateOne(
            new CreateIndexModel<AuditLog>(
                Builders<AuditLog>.IndexKeys
                    .Ascending(x => x.AggregateType)
                    .Ascending(x => x.AggregateId)));

        AuditLogs.Indexes.CreateOne(
            new CreateIndexModel<AuditLog>(
                Builders<AuditLog>.IndexKeys
                    .Descending(x => x.OccurredAt)));

        // DashboardReports — unique index on ReportKey
        // ensures only one dashboard document ever exists
        DashboardReports.Indexes.CreateOne(
            new CreateIndexModel<DashboardReport>(
                Builders<DashboardReport>.IndexKeys
                    .Ascending(x => x.ReportKey),
                new CreateIndexOptions { Unique = true }));
    }
}