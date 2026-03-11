namespace WorkforceAPI.Application.DTOs;

public class DashboardReportDto
{
    public DateTime GeneratedAt { get; set; }
    public HeadcountStatsDto Headcount { get; set; } = new();
    public ProjectStatsDto Projects { get; set; } = new();
    public LeaveStatsDto Leave { get; set; } = new();
    public List<RecentActivityDto> RecentActivity { get; set; } = [];
}

public class HeadcountStatsDto
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int Inactive { get; set; }
    public List<DepartmentHeadcountDto> ByDepartment { get; set; } = [];
}

public class DepartmentHeadcountDto
{
    public string Department { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ProjectStatsDto
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int OnHold { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
}

public class LeaveStatsDto
{
    public int TotalRequests { get; set; }
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int Cancelled { get; set; }
}

public class RecentActivityDto
{
    public string EventType { get; set; } = string.Empty;
    public string AggregateType { get; set; } = string.Empty;
    public int AggregateId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
}