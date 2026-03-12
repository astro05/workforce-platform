using WorkforceAPI.Domain.Entities;

namespace WorkforceAPI.Domain.Repositories;

public interface ILeaveRequestRepository
{
    Task<LeaveRequest?> GetByIdAsync(string id, CancellationToken ct = default);

    Task<IEnumerable<LeaveRequest>> GetAllAsync(CancellationToken ct = default);

    Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(int employeeId, CancellationToken ct = default);

    Task<IEnumerable<LeaveRequest>> GetFilteredAsync(string? status = null, string? leaveType = null, int? employeeId = null, CancellationToken ct = default);

    Task<LeaveRequest> CreateAsync(LeaveRequest request, CancellationToken ct = default);

    Task UpdateAsync(LeaveRequest request, CancellationToken ct = default);
}