using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WorkforceAPI.Domain.Entities;

public class LeaveRequest
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = LeaveStatus.Pending;
    public string? Reason { get; set; }
    public List<ApprovalEntry> ApprovalHistory { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class ApprovalEntry
{
    public string Status { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public string ActorName { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}

public static class LeaveStatus
{
    public const string Pending = "Pending";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string Cancelled = "Cancelled";
}

public static class LeaveType
{
    public const string Sick = "Sick";
    public const string Casual = "Casual";
    public const string Annual = "Annual";
    public const string Unpaid = "Unpaid";
}