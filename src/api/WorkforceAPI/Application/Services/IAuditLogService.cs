using WorkforceAPI.Application.DTOs;

namespace WorkforceAPI.Application.Services;

public interface IAuditLogService
{
    Task<IEnumerable<AuditLogDto>> GetRecentAsync(int limit = 50, CancellationToken ct = default);

    Task<IEnumerable<AuditLogDto>> GetByEntityAsync(string aggregateType, int aggregateId, CancellationToken ct = default);

    Task InsertAsync(CreateAuditLogDto dto, CancellationToken ct = default);
}