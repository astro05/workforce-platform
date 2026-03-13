using MongoDB.Bson;
using MongoDB.Driver;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Infrastructure.Persistence.MongoDb;

public class DashboardReportRepository : IDashboardReportRepository
{
    private readonly IMongoCollection<BsonDocument> _rawCollection;
    private readonly IMongoCollection<DashboardReport> _collection;

    public DashboardReportRepository(MongoDbContext ctx)
    {
        _rawCollection = ctx.Database
            .GetCollection<BsonDocument>("DashboardReports");
        _collection = ctx.DashboardReports;
    }

    public async Task<DashboardReport?> GetLatestAsync(
        CancellationToken ct = default)
    {
        // Read as raw BsonDocument to handle
        // camelCase from Node.js worker
        var doc = await _rawCollection
            .Find(Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter
                    .Eq("reportKey", "dashboard"),
                Builders<BsonDocument>.Filter
                    .Eq("ReportKey", "dashboard")))
            .FirstOrDefaultAsync(ct);

        if (doc is null) return null;

        return MapFromBson(doc);
    }

    public async Task UpsertAsync(
        DashboardReport report,
        CancellationToken ct = default)
    {
        report.GeneratedAt = DateTime.UtcNow;
        report.ReportKey = "dashboard";

        await _collection.ReplaceOneAsync(
            x => x.ReportKey == "dashboard",
            report,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }

    // ── Map BsonDocument → DashboardReport ───────────────────
    private static DashboardReport MapFromBson(BsonDocument doc)
    {
        static BsonValue Get(BsonDocument d,
            string pascal, string camel)
            => d.Contains(pascal) ? d[pascal]
             : d.Contains(camel) ? d[camel]
             : BsonNull.Value;

        static int GetInt(BsonDocument d,
            string pascal, string camel)
        {
            var v = Get(d, pascal, camel);
            return v == BsonNull.Value ? 0 : v.ToInt32();
        }

        static string GetStr(BsonDocument d,
            string pascal, string camel)
        {
            var v = Get(d, pascal, camel);
            return v == BsonNull.Value
                ? string.Empty : v.AsString;
        }

        // Headcount
        var hRaw = Get(doc, "Headcount", "headcount");
        var hDoc = hRaw == BsonNull.Value
            ? new BsonDocument() : hRaw.AsBsonDocument;

        var byDept = new List<DepartmentHeadcount>();
        var byDeptRaw = Get(hDoc, "ByDepartment",
            "byDepartment");
        if (byDeptRaw != BsonNull.Value)
        {
            foreach (var item in byDeptRaw.AsBsonArray)
            {
                var d = item.AsBsonDocument;
                byDept.Add(new DepartmentHeadcount
                {
                    Department = GetStr(d,
                        "Department", "department"),
                    Count = GetInt(d, "Count", "count"),
                });
            }
        }

        // Projects
        var pRaw = Get(doc, "Projects", "projects");
        var pDoc = pRaw == BsonNull.Value
            ? new BsonDocument() : pRaw.AsBsonDocument;

        // Leave
        var lRaw = Get(doc, "Leave", "leave");
        var lDoc = lRaw == BsonNull.Value
            ? new BsonDocument() : lRaw.AsBsonDocument;

        // Recent Activity
        var activity = new List<RecentActivity>();
        var raRaw = Get(doc, "RecentActivity",
            "recentActivity");
        if (raRaw != BsonNull.Value)
        {
            foreach (var item in raRaw.AsBsonArray)
            {
                var d = item.AsBsonDocument;
                var oaRaw = Get(d, "OccurredAt",
                    "occurredAt");
                activity.Add(new RecentActivity
                {
                    EventType = GetStr(d,
                        "EventType", "eventType"),
                    AggregateType = GetStr(d,
                        "AggregateType", "aggregateType"),
                    AggregateId = GetInt(d,
                        "AggregateId", "aggregateId"),
                    Description = GetStr(d,
                        "Description", "description"),
                    OccurredAt = oaRaw == BsonNull.Value
                        ? DateTime.UtcNow
                        : oaRaw.ToUniversalTime(),
                });
            }
        }

        // GeneratedAt
        var gaRaw = Get(doc, "GeneratedAt", "generatedAt");
        var generatedAt = gaRaw == BsonNull.Value
            ? DateTime.UtcNow : gaRaw.ToUniversalTime();

        return new DashboardReport
        {
            Id = doc["_id"].ToString()!,
            ReportKey = "dashboard",
            GeneratedAt = generatedAt,
            Headcount = new HeadcountStats
            {
                Total = GetInt(hDoc, "Total",
                    "total"),
                Active = GetInt(hDoc, "Active",
                    "active"),
                Inactive = GetInt(hDoc, "Inactive",
                    "inactive"),
                ByDepartment = byDept,
            },
            Projects = new ProjectStats
            {
                Total = GetInt(pDoc,
                    "Total", "total"),
                Active = GetInt(pDoc,
                    "Active", "active"),
                OnHold = GetInt(pDoc,
                    "OnHold", "onHold"),
                Completed = GetInt(pDoc,
                    "Completed", "completed"),
                Cancelled = GetInt(pDoc,
                    "Cancelled", "cancelled"),
                TotalTasks = GetInt(pDoc,
                    "TotalTasks", "totalTasks"),
                CompletedTasks = GetInt(pDoc,
                    "CompletedTasks", "completedTasks"),
            },
            Leave = new LeaveStats
            {
                TotalRequests = GetInt(lDoc,
                    "TotalRequests", "totalRequests"),
                Pending = GetInt(lDoc,
                    "Pending", "pending"),
                Approved = GetInt(lDoc,
                    "Approved", "approved"),
                Rejected = GetInt(lDoc,
                    "Rejected", "rejected"),
                Cancelled = GetInt(lDoc,
                    "Cancelled", "cancelled"),
            },
            RecentActivity = activity,
        };
    }
}