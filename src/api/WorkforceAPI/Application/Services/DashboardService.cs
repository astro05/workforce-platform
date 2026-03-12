using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Domain.Repositories;
namespace WorkforceAPI.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardReportRepository _repo;

    public DashboardService(IDashboardReportRepository repo)
    {
        _repo = repo;
    }

    public async Task<DashboardReportDto?> GetReportAsync(
        CancellationToken ct = default)
    {
        var report = await _repo.GetLatestAsync(ct);

        if (report is null) return null;

        return new DashboardReportDto
        {
            GeneratedAt = report.GeneratedAt,
            Headcount = new HeadcountStatsDto
            {
                Total = report.Headcount.Total,
                Active = report.Headcount.Active,
                Inactive = report.Headcount.Inactive,
                ByDepartment = report.Headcount.ByDepartment
                    .Select(d => new DepartmentHeadcountDto
                    {
                        Department = d.Department,
                        Count = d.Count
                    }).ToList()
            },
            Projects = new ProjectStatsDto
            {
                Total = report.Projects.Total,
                Active = report.Projects.Active,
                OnHold = report.Projects.OnHold,
                Completed = report.Projects.Completed,
                Cancelled = report.Projects.Cancelled,
                TotalTasks = report.Projects.TotalTasks,
                CompletedTasks = report.Projects.CompletedTasks
            },
            Leave = new LeaveStatsDto
            {
                TotalRequests = report.Leave.TotalRequests,
                Pending = report.Leave.Pending,
                Approved = report.Leave.Approved,
                Rejected = report.Leave.Rejected,
                Cancelled = report.Leave.Cancelled
            },
            RecentActivity = report.RecentActivity
                .Select(a => new RecentActivityDto
                {
                    EventType = a.EventType,
                    AggregateType = a.AggregateType,
                    AggregateId = a.AggregateId,
                    Description = a.Description,
                    OccurredAt = a.OccurredAt
                }).ToList()
        };
    }
}