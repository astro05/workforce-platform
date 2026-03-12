using MongoDB.Driver;

namespace AuditWorker.Models;

public class AuditMongoContext
{
    private readonly IMongoDatabase _database;

    public AuditMongoContext(IConfiguration config)
    {
        var connectionString = config
            .GetConnectionString("Mongo")
            ?? throw new InvalidOperationException(
                "MongoDB connection string not configured.");

        var databaseName = config
            .GetConnectionString("MongoDatabaseName")
            ?? "WorkforceDb";

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);

        EnsureIndexes();
    }

    public IMongoCollection<AuditLogDocument> AuditLogs
        => _database.GetCollection<AuditLogDocument>("AuditLogs");

    private void EnsureIndexes()
    {
        // Index on AggregateType + AggregateId
        AuditLogs.Indexes.CreateOne(
            new CreateIndexModel<AuditLogDocument>(
                Builders<AuditLogDocument>.IndexKeys
                    .Ascending(x => x.AggregateType)
                    .Ascending(x => x.AggregateId)));

        // Index on OccurredAt descending
        AuditLogs.Indexes.CreateOne(
            new CreateIndexModel<AuditLogDocument>(
                Builders<AuditLogDocument>.IndexKeys
                    .Descending(x => x.OccurredAt)));
    }
}