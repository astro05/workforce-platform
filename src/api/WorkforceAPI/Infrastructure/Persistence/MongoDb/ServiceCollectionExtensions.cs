using WorkforceAPI.Application.Services;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Infrastructure.Persistence.MongoDb;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDbPersistence(
        this IServiceCollection services)
    {
        // ── MongoDB Context (Singleton) ───────────────────────
        services.AddSingleton<MongoDbContext>();

        // ── MongoDB Repositories (Singleton) ──────────────────
        services.AddSingleton<ILeaveRequestRepository, LeaveRequestRepository>();
        services.AddSingleton<IAuditLogRepository, AuditLogRepository>();
        services.AddSingleton<IDashboardReportRepository, DashboardReportRepository>();

        // ── MongoDB Application Services ──────────────────────
        services.AddSingleton<ILeaveRequestService, LeaveRequestService>();
        services.AddSingleton<IAuditLogService, AuditLogService>();
        services.AddSingleton<IDashboardService, DashboardService>();

        return services;
    }
}