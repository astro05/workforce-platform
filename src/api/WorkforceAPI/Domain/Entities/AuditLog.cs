using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WorkforceAPI.Domain.Entities;

public class AuditLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string EventType { get; set; } = string.Empty;
    public string AggregateType { get; set; } = string.Empty;
    public int AggregateId { get; set; }
    public string? ActorName { get; set; }
    public string Description { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.String)]
    public string? Before { get; set; }

    [BsonRepresentation(BsonType.String)]
    public string? After { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}