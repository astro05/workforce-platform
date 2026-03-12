using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Events;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repo;
    private readonly IEventPublisher _publisher;

    public ProjectService(
        IProjectRepository repo,
        IEventPublisher publisher)
    {
        _repo = repo;
        _publisher = publisher;
    }

    // ── Get All ───────────────────────────────────────────────
    public async Task<IEnumerable<ProjectDto>> GetAllAsync(
        CancellationToken ct = default)
    {
        var projects = await _repo.GetAllWithDetailsAsync(ct);
        return projects.Select(MapToDto);
    }

    // ── Get By Id ─────────────────────────────────────────────
    public async Task<ProjectDto?> GetByIdAsync(
        int id,
        CancellationToken ct = default)
    {
        var project = await _repo.GetWithDetailsAsync(id, ct);
        return project is null ? null : MapToDto(project);
    }

    // ── Create ────────────────────────────────────────────────
    public async Task<ProjectDto> CreateAsync(
        CreateProjectDto dto,
        CancellationToken ct = default)
    {
        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            Status = dto.Status,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _repo.AddAsync(project, ct);
        await _repo.SaveChangesAsync(ct);

        // Add initial members if provided
        foreach (var memberId in dto.MemberIds)
        {
            await _repo.AddMemberAsync(
                project.Id, memberId, ct: ct);
        }

        if (dto.MemberIds.Count > 0)
            await _repo.SaveChangesAsync(ct);

        // Publish domain event
        await _publisher.PublishAsync(
            new ProjectCreatedEvent(
                project.Id,
                project.Name,
                project.Status.ToString()),
            ct);

        // Reload with full details
        var created = await _repo
            .GetWithDetailsAsync(project.Id, ct);

        return MapToDto(created!);
    }

    // ── Update ────────────────────────────────────────────────
    public async Task<ProjectDto> UpdateAsync(
        int id,
        UpdateProjectDto dto,
        CancellationToken ct = default)
    {
        var project = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException(
                $"Project {id} not found.");

        // Capture before snapshot
        var before = new
        {
            project.Name,
            project.Description,
            Status = project.Status.ToString(),
            project.StartDate,
            project.EndDate,
        };

        // Apply changes
        project.Name = dto.Name;
        project.Description = dto.Description;
        project.Status = dto.Status;
        project.StartDate = dto.StartDate;
        project.EndDate = dto.EndDate;
        project.UpdatedAt = DateTime.UtcNow;

        // Capture after snapshot
        var after = new
        {
            project.Name,
            project.Description,
            Status = project.Status.ToString(),
            project.StartDate,
            project.EndDate,
        };

        await _repo.UpdateAsync(project, ct);
        await _repo.SaveChangesAsync(ct);

        // Publish domain event
        await _publisher.PublishAsync(
            new ProjectUpdatedEvent(
                project.Id,
                project.Name,
                project.Status.ToString(),
                before,
                after),
            ct);

        // Reload with full details
        var updated = await _repo
            .GetWithDetailsAsync(id, ct);

        return MapToDto(updated!);
    }

    // ── Add Member ────────────────────────────────────────────
    public async Task AddMemberAsync(
        int projectId,
        int employeeId,
        string role,
        CancellationToken ct = default)
    {
        await _repo.AddMemberAsync(
            projectId, employeeId, role, ct);
        await _repo.SaveChangesAsync(ct);
    }

    // ── Remove Member ─────────────────────────────────────────
    public async Task RemoveMemberAsync(
        int projectId,
        int employeeId,
        CancellationToken ct = default)
    {
        await _repo.RemoveMemberAsync(
            projectId, employeeId, ct);
        await _repo.SaveChangesAsync(ct);
    }

    // ── Mapping ───────────────────────────────────────────────
    private static ProjectDto MapToDto(Project p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Status = p.Status.ToString(),
        StartDate = p.StartDate,
        EndDate = p.EndDate,
        MemberCount = p.ProjectEmployees.Count,
        TaskCount = p.Tasks.Count,
        CompletedTaskCount = p.Tasks.Count(
            t => t.Status == TaskItemStatus.Done),
        Members = p.ProjectEmployees.Select(pe =>
            new ProjectMemberDto
            {
                EmployeeId = pe.Employee.Id,
                FullName = pe.Employee.FullName,
                Email = pe.Employee.Email,
                Role = pe.Role,
                AvatarUrl = pe.Employee.AvatarUrl,
                DepartmentName = pe.Employee
                    .Department?.Name ?? string.Empty,
            }).ToList(),
        Tasks = p.Tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status.ToString(),
            Priority = t.Priority.ToString(),
            DueDate = t.DueDate,
            ProjectId = t.ProjectId,
            AssignedToId = t.AssignedToId,
            AssignedToName = t.AssignedTo?.FullName,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
        }).ToList(),
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt,
    };
}