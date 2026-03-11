using Microsoft.AspNetCore.Mvc;
using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Application.Services;

namespace WorkforceAPI.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _service;
    private readonly ILogger<AuditLogsController> _logger;

    public AuditLogsController(
        IAuditLogService service,
        ILogger<AuditLogsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // ── GET /api/v1/auditlogs ─────────────────────────────────
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecent(
        [FromQuery] AuditLogQueryParams query,
        CancellationToken ct)
    {
        // If entity filter provided — return by entity
        if (!string.IsNullOrWhiteSpace(query.AggregateType)
            && query.AggregateId.HasValue)
        {
            var entityLogs = await _service.GetByEntityAsync(
                query.AggregateType,
                query.AggregateId.Value,
                ct);

            return Ok(entityLogs);
        }

        // Otherwise return recent logs
        var logs = await _service.GetRecentAsync(query.Limit, ct);
        return Ok(logs);
    }

    // ── GET /api/v1/auditlogs/{aggregateType}/{aggregateId} ───
    [HttpGet("{aggregateType}/{aggregateId:int}")]
    [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEntity(
        string aggregateType,
        int aggregateId,
        CancellationToken ct)
    {
        var logs = await _service.GetByEntityAsync(
            aggregateType, aggregateId, ct);

        return Ok(logs);
    }

    // ── POST /api/v1/auditlogs ────────────────────────────────
    // Used internally by workers — not exposed to frontend directly
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Insert(
        [FromBody] CreateAuditLogDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.InsertAsync(dto, ct);
        _logger.LogInformation(
            "Audit log inserted: {EventType} on {AggregateType} {AggregateId}",
            dto.EventType, dto.AggregateType, dto.AggregateId);

        return NoContent();
    }
}