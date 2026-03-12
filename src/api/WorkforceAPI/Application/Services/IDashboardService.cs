using WorkforceAPI.Application.DTOs;

namespace WorkforceAPI.Application.Services;

public interface IDashboardService
{
    Task<DashboardReportDto?> GetReportAsync(CancellationToken ct = default);
}