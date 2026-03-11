using MongoDB.Driver;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Infrastructure.Persistence.MongoDb;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly IMongoCollection<AuditLog> _collection;

    public AuditLogRepository(MongoDbContext ctx)
    {
        _collection = ctx.AuditLogs;
    }

    public async Task<IEnumerable<AuditLog>> GetRecentAsync(
        int limit = 50, CancellationToken ct = default)
        => await _collection
            .Find(Builders<AuditLog>.Filter.Empty)
            .SortByDescending(x => x.OccurredAt)
            .Limit(limit)
            .ToListAsync(ct);

    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(
        string aggregateType, int aggregateId, CancellationToken ct = default)
        => await _collection
            .Find(x =>
                x.AggregateType == aggregateType &&
                x.AggregateId == aggregateId)
            .SortByDescending(x => x.OccurredAt)
            .ToListAsync(ct);

    public async Task InsertAsync(
        AuditLog log, CancellationToken ct = default)
        => await _collection.InsertOneAsync(log, null, ct);
}