using System.ComponentModel.DataAnnotations;

namespace WorkforceAPI.Application.DTOs;

public class LeaveRequestDto
{
    public string Id { get; set; } = string.Empty;
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public List<ApprovalEntryDto> ApprovalHistory { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ApprovalEntryDto
{
    public string Status { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public string ActorName { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
}

public class CreateLeaveRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "EmployeeId is required.")]
    public int EmployeeId { get; set; }

    [Required]
    [MaxLength(100)]
    public string EmployeeName { get; set; } = string.Empty;

    [Required]
    public string LeaveType { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }
}

public class UpdateLeaveStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Comment { get; set; }

    [Required]
    [MaxLength(100)]
    public string ActorName { get; set; } = string.Empty;
}

public class LeaveQueryParams
{
    public string? Status { get; set; }
    public string? LeaveType { get; set; }
    public int? EmployeeId { get; set; }
}