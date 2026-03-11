namespace WorkforceAPI.Application.DTOs;

public class AuditLogDto
{
    public string Id { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string AggregateType { get; set; } = string.Empty;
    public int AggregateId { get; set; }
    public string? ActorName { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Before { get; set; }
    public string? After { get; set; }
    public DateTime OccurredAt { get; set; }
}

public class AuditLogQueryParams
{
    public string? AggregateType { get; set; }
    public int? AggregateId { get; set; }
    public int Limit { get; set; } = 50;
}

public class CreateAuditLogDto
{
    public string EventType { get; set; } = string.Empty;
    public string AggregateType { get; set; } = string.Empty;
    public int AggregateId { get; set; }
    public string? ActorName { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Before { get; set; }
    public string? After { get; set; }
}