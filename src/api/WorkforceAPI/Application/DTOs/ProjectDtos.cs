using WorkforceAPI.Domain.Entities;

namespace WorkforceAPI.Application.DTOs;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int MemberCount { get; set; }
    public int TaskCount { get; set; }
    public int CompletedTaskCount { get; set; }
    public List<ProjectMemberDto> Members { get; set; } = [];
    public List<TaskDto> Tasks { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ProjectMemberDto
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
}

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<int> MemberIds { get; set; } = [];
}

public class UpdateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}