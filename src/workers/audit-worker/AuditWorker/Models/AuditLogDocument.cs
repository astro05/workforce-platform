using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuditWorker.Models;

public class AuditLogDocument
{
    // Use EventId as _id for idempotency
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    public string EventType { get; set; } = string.Empty;
    public string AggregateType { get; set; } = string.Empty;
    public int AggregateId { get; set; }
    public string? ActorName { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Before { get; set; }
    public string? After { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}