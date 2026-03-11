using MongoDB.Driver;
using WorkforceAPI.Domain.Entities;

namespace WorkforceAPI.Infrastructure.Persistence.MongoDb;

public static class MongoDbSeeder
{
    public static async Task SeedAsync(MongoDbContext ctx)
    {
        // Skip if already seeded
        var count = await ctx.LeaveRequests
            .CountDocumentsAsync(_ => true);

        if (count > 0) return;

        var leaveRequests = new List<LeaveRequest>
        {
            new()
            {
                EmployeeId   = 1,
                EmployeeName = "James Anderson",
                LeaveType    = LeaveType.Annual,
                StartDate    = new DateTime(2024, 12, 23),
                EndDate      = new DateTime(2024, 12, 27),
                Status       = LeaveStatus.Approved,
                Reason       = "Christmas holiday",
                ApprovalHistory =
                [
                    new()
                    {
                        Status    = LeaveStatus.Pending,
                        ActorName = "James Anderson",
                        Comment   = "Requesting annual leave for Christmas",
                        ChangedAt = new DateTime(2024, 12, 1)
                    },
                    new()
                    {
                        Status    = LeaveStatus.Approved,
                        ActorName = "Jonathan Lewis",
                        Comment   = "Approved. Enjoy the holidays!",
                        ChangedAt = new DateTime(2024, 12, 3)
                    }
                ]
            },
            new()
            {
                EmployeeId   = 2,
                EmployeeName = "Sarah Mitchell",
                LeaveType    = LeaveType.Sick,
                StartDate    = new DateTime(2024, 11, 4),
                EndDate      = new DateTime(2024, 11, 5),
                Status       = LeaveStatus.Approved,
                Reason       = "Flu and fever",
                ApprovalHistory =
                [
                    new()
                    {
                        Status    = LeaveStatus.Pending,
                        ActorName = "Sarah Mitchell",
                        Comment   = "Not feeling well",
                        ChangedAt = new DateTime(2024, 11, 4)
                    },
                    new()
                    {
                        Status    = LeaveStatus.Approved,
                        ActorName = "Jonathan Lewis",
                        Comment   = "Get well soon",
                        ChangedAt = new DateTime(2024, 11, 4)
                    }
                ]
            },
            new()
            {
                EmployeeId   = 3,
                EmployeeName = "David Chen",
                LeaveType    = LeaveType.Casual,
                StartDate    = new DateTime(2025, 1, 10),
                EndDate      = new DateTime(2025, 1, 10),
                Status       = LeaveStatus.Pending,
                Reason       = "Personal errand",
                ApprovalHistory =
                [
                    new()
                    {
                        Status    = LeaveStatus.Pending,
                        ActorName = "David Chen",
                        Comment   = "Need a day off for personal work",
                        ChangedAt = new DateTime(2025, 1, 5)
                    }
                ]
            },
            new()
            {
                EmployeeId   = 4,
                EmployeeName = "Emily Johnson",
                LeaveType    = LeaveType.Annual,
                StartDate    = new DateTime(2025, 2, 3),
                EndDate      = new DateTime(2025, 2, 7),
                Status       = LeaveStatus.Rejected,
                Reason       = "Family vacation",
                ApprovalHistory =
                [
                    new()
                    {
                        Status    = LeaveStatus.Pending,
                        ActorName = "Emily Johnson",
                        Comment   = "Family vacation planned",
                        ChangedAt = new DateTime(2025, 1, 20)
                    },
                    new()
                    {
                        Status    = LeaveStatus.Rejected,
                        ActorName = "Jonathan Lewis",
                        Comment   = "Project deadline conflicts",
                        ChangedAt = new DateTime(2025, 1, 22)
                    }
                ]
            },
            new()
            {
                EmployeeId   = 5,
                EmployeeName = "Michael Brown",
                LeaveType    = LeaveType.Unpaid,
                StartDate    = new DateTime(2025, 3, 1),
                EndDate      = new DateTime(2025, 3, 5),
                Status       = LeaveStatus.Pending,
                Reason       = "Extended personal leave",
                ApprovalHistory =
                [
                    new()
                    {
                        Status    = LeaveStatus.Pending,
                        ActorName = "Michael Brown",
                        Comment   = "Need unpaid leave for personal reasons",
                        ChangedAt = new DateTime(2025, 2, 15)
                    }
                ]
            },
            new()
            {
                EmployeeId   = 6,
                EmployeeName = "Jessica Taylor",
                LeaveType    = LeaveType.Sick,
                StartDate    = new DateTime(2024, 10, 14),
                EndDate      = new DateTime(2024, 10, 15),
                Status       = LeaveStatus.Approved,
                Reason       = "Medical appointment",
                ApprovalHistory =
                [
                    new()
                    {
                        Status    = LeaveStatus.Pending,
                        ActorName = "Jessica Taylor",
                        Comment   = "Scheduled medical checkup",
                        ChangedAt = new DateTime(2024, 10, 10)
                    },
                    new()
                    {
                        Status    = LeaveStatus.Approved,
                        ActorName = "Jonathan Lewis",
                        Comment   = "Approved",
                        ChangedAt = new DateTime(2024, 10, 11)
                    }
                ]
            },
            new()
            {
                EmployeeId   = 7,
                EmployeeName = "Daniel Wilson",
                LeaveType    = LeaveType.Casual,
                StartDate    = new DateTime(2025, 1, 20),
                EndDate      = new DateTime(2025, 1, 20),
                Status       = LeaveStatus.Cancelled,
                Reason       = "Moving apartments",
                ApprovalHistory =
                [
                    new()
                    {
                        Status    = LeaveStatus.Pending,
                        ActorName = "Daniel Wilson",
                        Comment   = "Need help moving",
                        ChangedAt = new DateTime(2025, 1, 15)
                    },
                    new()
                    {
                        Status    = LeaveStatus.Cancelled,
                        ActorName = "Daniel Wilson",
                        Comment   = "Moving postponed",
                        ChangedAt = new DateTime(2025, 1, 17)
                    }
                ]
            },
            new()
            {
                EmployeeId   = 8,
                EmployeeName = "Ashley Moore",
                LeaveType    = LeaveType.Annual,
                StartDate    = new DateTime(2025, 4, 14),
                EndDate      = new DateTime(2025, 4, 18),
                Status       = LeaveStatus.Pending,
                Reason       = "Spring break trip",
                ApprovalHistory =
                [
                    new()
                    {
                        Status    = LeaveStatus.Pending,
                        ActorName = "Ashley Moore",
                        Comment   = "Planned vacation",
                        ChangedAt = new DateTime(2025, 3, 1)
                    }
                ]
            }
        };

        await ctx.LeaveRequests.InsertManyAsync(leaveRequests);
        Console.WriteLine("MongoDB seed data inserted successfully");
    }
}