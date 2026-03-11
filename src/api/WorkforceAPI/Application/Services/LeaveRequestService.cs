using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Application.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _repo;

    public LeaveRequestService(ILeaveRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<LeaveRequestDto>> GetAllAsync(
        LeaveQueryParams query, CancellationToken ct = default)
    {
        var requests = await _repo.GetFilteredAsync(
            query.Status,
            query.LeaveType,
            query.EmployeeId,
            ct);

        return requests.Select(MapToDto);
    }

    public async Task<IEnumerable<LeaveRequestDto>> GetByEmployeeAsync(
        int employeeId, CancellationToken ct = default)
    {
        var requests = await _repo.GetByEmployeeAsync(employeeId, ct);
        return requests.Select(MapToDto);
    }

    public async Task<LeaveRequestDto?> GetByIdAsync(
        string id, CancellationToken ct = default)
    {
        var request = await _repo.GetByIdAsync(id, ct);
        return request is null ? null : MapToDto(request);
    }

    public async Task<LeaveRequestDto> CreateAsync(
        CreateLeaveRequestDto dto, CancellationToken ct = default)
    {
        // Validate dates
        if (dto.EndDate < dto.StartDate)
            throw new InvalidOperationException(
                "EndDate must be on or after StartDate.");

        var request = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            EmployeeName = dto.EmployeeName,
            LeaveType = dto.LeaveType,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = LeaveStatus.Pending,
            Reason = dto.Reason,
            ApprovalHistory =
            [
                new ApprovalEntry
                {
                    Status    = LeaveStatus.Pending,
                    ActorName = dto.EmployeeName,
                    Comment   = dto.Reason ?? "Leave request submitted",
                    ChangedAt = DateTime.UtcNow
                }
            ]
        };

        var created = await _repo.CreateAsync(request, ct);
        return MapToDto(created);
    }

    public async Task<LeaveRequestDto> UpdateStatusAsync(
        string id, UpdateLeaveStatusDto dto, CancellationToken ct = default)
    {
        var request = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException(
                $"Leave request {id} not found.");

        // Validate status transition
        ValidateStatusTransition(request.Status, dto.Status);

        request.Status = dto.Status;
        request.UpdatedAt = DateTime.UtcNow;

        // Append to approval history
        request.ApprovalHistory.Add(new ApprovalEntry
        {
            Status = dto.Status,
            ActorName = dto.ActorName,
            Comment = dto.Comment,
            ChangedAt = DateTime.UtcNow
        });

        await _repo.UpdateAsync(request, ct);
        return MapToDto(request);
    }

    public async Task CancelAsync(
        string id, string actorName, CancellationToken ct = default)
    {
        var request = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException(
                $"Leave request {id} not found.");

        if (request.Status == LeaveStatus.Approved ||
            request.Status == LeaveStatus.Rejected)
            throw new InvalidOperationException(
                $"Cannot cancel a request that is already {request.Status}.");

        request.Status = LeaveStatus.Cancelled;
        request.UpdatedAt = DateTime.UtcNow;

        request.ApprovalHistory.Add(new ApprovalEntry
        {
            Status = LeaveStatus.Cancelled,
            ActorName = actorName,
            Comment = "Request cancelled",
            ChangedAt = DateTime.UtcNow
        });

        await _repo.UpdateAsync(request, ct);
    }

    // ── Helpers ───────────────────────────────────────────────
    private static void ValidateStatusTransition(
        string current, string next)
    {
        var allowed = current switch
        {
            LeaveStatus.Pending => new[] { LeaveStatus.Approved, LeaveStatus.Rejected, LeaveStatus.Cancelled },
            LeaveStatus.Approved => new[] { LeaveStatus.Cancelled },
            _ => Array.Empty<string>()
        };

        if (!allowed.Contains(next))
            throw new InvalidOperationException(
                $"Cannot transition from '{current}' to '{next}'.");
    }

    // ── Mapping ───────────────────────────────────────────────
    private static LeaveRequestDto MapToDto(LeaveRequest r) => new()
    {
        Id = r.Id,
        EmployeeId = r.EmployeeId,
        EmployeeName = r.EmployeeName,
        LeaveType = r.LeaveType,
        StartDate = r.StartDate,
        EndDate = r.EndDate,
        TotalDays = (int)(r.EndDate - r.StartDate).TotalDays + 1,
        Status = r.Status,
        Reason = r.Reason,
        ApprovalHistory = r.ApprovalHistory.Select(a => new ApprovalEntryDto
        {
            Status = a.Status,
            Comment = a.Comment,
            ActorName = a.ActorName,
            ChangedAt = a.ChangedAt
        }).ToList(),
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };
}