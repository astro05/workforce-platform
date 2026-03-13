using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WorkforceAPI.Domain.Entities;

public class DashboardReport
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    public string ReportKey { get; set; } = "dashboard";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public HeadcountStats Headcount { get; set; } = new();
    public ProjectStats Projects { get; set; } = new();
    public LeaveStats Leave { get; set; } = new();
    public List<RecentActivity> RecentActivity { get; set; } = [];
}

public class HeadcountStats
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int Inactive { get; set; }
    public List<DepartmentHeadcount> ByDepartment { get; set; } = [];
}

public class DepartmentHeadcount
{
    public string Department { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ProjectStats
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int OnHold { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
}

public class LeaveStats
{
    public int TotalRequests { get; set; }
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int Cancelled { get; set; }
}

public class RecentActivity
{
    public string EventType { get; set; } = string.Empty;
    public string AggregateType { get; set; } = string.Empty;
    public int AggregateId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
}