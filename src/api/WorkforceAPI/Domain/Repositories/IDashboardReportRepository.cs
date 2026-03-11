using WorkforceAPI.Domain.Entities;

namespace WorkforceAPI.Domain.Repositories;

public interface IDashboardReportRepository
{
    Task<DashboardReport?> GetLatestAsync(
        CancellationToken ct = default);

    Task UpsertAsync(
        DashboardReport report,
        CancellationToken ct = default);
}