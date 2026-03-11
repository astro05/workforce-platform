using WorkforceAPI.Domain.Entities;

namespace WorkforceAPI.Domain.Repositories;

public interface IAuditLogRepository
{
    Task<IEnumerable<AuditLog>> GetRecentAsync(
        int limit = 50,
        CancellationToken ct = default);

    Task<IEnumerable<AuditLog>> GetByEntityAsync(
        string aggregateType,
        int aggregateId,
        CancellationToken ct = default);

    Task InsertAsync(
        AuditLog log,
        CancellationToken ct = default);
}