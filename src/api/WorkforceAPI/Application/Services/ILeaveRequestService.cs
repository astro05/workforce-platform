using WorkforceAPI.Application.DTOs;

namespace WorkforceAPI.Application.Services;

public interface ILeaveRequestService
{
    Task<IEnumerable<LeaveRequestDto>> GetAllAsync(LeaveQueryParams query, CancellationToken ct = default);

    Task<IEnumerable<LeaveRequestDto>> GetByEmployeeAsync(int employeeId, CancellationToken ct = default);

    Task<LeaveRequestDto?> GetByIdAsync(string id, CancellationToken ct = default);

    Task<LeaveRequestDto> CreateAsync(CreateLeaveRequestDto dto, CancellationToken ct = default);

    Task<LeaveRequestDto> UpdateStatusAsync(string id, UpdateLeaveStatusDto dto, CancellationToken ct = default);

    Task CancelAsync(string id, string actorName, CancellationToken ct = default);
}