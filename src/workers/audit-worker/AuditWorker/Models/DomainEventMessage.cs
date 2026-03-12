namespace AuditWorker.Models;

public class DomainEventMessage
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string AggregateType { get; set; } = string.Empty;
    public int AggregateId { get; set; }
    public string? ActorName { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Before { get; set; }
    public string? After { get; set; }
    public DateTime OccurredAt { get; set; }
}