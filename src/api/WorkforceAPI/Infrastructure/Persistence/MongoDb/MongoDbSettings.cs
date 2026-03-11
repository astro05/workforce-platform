namespace WorkforceAPI.Infrastructure.Persistence.MongoDb;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "WorkforceDb";
    public string LeaveRequestsCollection { get; set; } = "LeaveRequests";
    public string AuditLogsCollection { get; set; } = "AuditLogs";
    public string DashboardReportsCollection { get; set; } = "DashboardReports";
}