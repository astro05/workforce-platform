using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repo;

    public AuditLogService(IAuditLogRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<AuditLogDto>> GetRecentAsync(
        int limit = 50, CancellationToken ct = default)
    {
        var logs = await _repo.GetRecentAsync(limit, ct);
        return logs.Select(MapToDto);
    }

    public async Task<IEnumerable<AuditLogDto>> GetByEntityAsync(
        string aggregateType,
        int aggregateId,
        CancellationToken ct = default)
    {
        var logs = await _repo.GetByEntityAsync(
            aggregateType, aggregateId, ct);
        return logs.Select(MapToDto);
    }

    public async Task InsertAsync(
        CreateAuditLogDto dto, CancellationToken ct = default)
    {
        var log = new AuditLog
        {
            EventType = dto.EventType,
            AggregateType = dto.AggregateType,
            AggregateId = dto.AggregateId,
            ActorName = dto.ActorName,
            Description = dto.Description,
            Before = dto.Before,
            After = dto.After,
            OccurredAt = DateTime.UtcNow
        };

        await _repo.InsertAsync(log, ct);
    }

    // ── Mapping ───────────────────────────────────────────────
    private static AuditLogDto MapToDto(AuditLog log) => new()
    {
        Id = log.Id,
        EventType = log.EventType,
        AggregateType = log.AggregateType,
        AggregateId = log.AggregateId,
        ActorName = log.ActorName,
        Description = log.Description,
        Before = log.Before,
        After = log.After,
        OccurredAt = log.OccurredAt
    };
}