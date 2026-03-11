using MongoDB.Driver;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Infrastructure.Persistence.MongoDb;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly IMongoCollection<LeaveRequest> _collection;

    public LeaveRequestRepository(MongoDbContext ctx)
    {
        _collection = ctx.LeaveRequests;
    }

    public async Task<LeaveRequest?> GetByIdAsync(
        string id, CancellationToken ct = default)
    {
        return await _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<LeaveRequest>> GetAllAsync(
        CancellationToken ct = default)
    {
        return await _collection
            .Find(_ => true)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(
        int employeeId, CancellationToken ct = default)
    {
        return await _collection
            .Find(x => x.EmployeeId == employeeId)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<LeaveRequest>> GetFilteredAsync(
        string? status = null,
        string? leaveType = null,
        int? employeeId = null,
        CancellationToken ct = default)
    {
        var builder = Builders<LeaveRequest>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(status))
            filter &= builder.Eq(x => x.Status, status);

        if (!string.IsNullOrWhiteSpace(leaveType))
            filter &= builder.Eq(x => x.LeaveType, leaveType);

        if (employeeId.HasValue)
            filter &= builder.Eq(x => x.EmployeeId, employeeId.Value);

        return await _collection
            .Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<LeaveRequest> CreateAsync(
        LeaveRequest request, CancellationToken ct = default)
    {
        await _collection.InsertOneAsync(request, null, ct);
        return request;
    }

    public async Task UpdateAsync(
        LeaveRequest request, CancellationToken ct = default)
    {
        request.UpdatedAt = DateTime.UtcNow;

        await _collection.ReplaceOneAsync(
            x => x.Id == request.Id,
            request,
            new ReplaceOptions { IsUpsert = false },
            ct);
    }
}