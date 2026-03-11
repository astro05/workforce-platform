using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;

    public TaskService(ITaskRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<TaskDto>> GetByProjectAsync(
        int projectId, CancellationToken ct = default)
    {
        var tasks = await _repo.GetByProjectAsync(projectId, ct);
        return tasks.Select(MapToDto);
    }

    public async Task<TaskDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var task = await _repo.GetByIdAsync(id, ct);
        return task is null ? null : MapToDto(task);
    }

    public async Task<TaskDto> CreateAsync(CreateTaskDto dto, CancellationToken ct = default)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            ProjectId = dto.ProjectId,
            AssignedToId = dto.AssignedToId
        };

        await _repo.AddAsync(task, ct);
        await _repo.SaveChangesAsync(ct);

        var created = await _repo.GetByIdAsync(task.Id, ct);
        return MapToDto(created!);
    }

    public async Task<TaskDto> UpdateAsync(
        int id, UpdateTaskDto dto, CancellationToken ct = default)
    {
        var task = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Task {id} not found.");

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;
        task.Priority = dto.Priority;
        task.DueDate = dto.DueDate;
        task.AssignedToId = dto.AssignedToId;
        task.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(task, ct);
        await _repo.SaveChangesAsync(ct);

        var updated = await _repo.GetByIdAsync(id, ct);
        return MapToDto(updated!);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var task = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Task {id} not found.");

        await _repo.DeleteAsync(task, ct);
        await _repo.SaveChangesAsync(ct);
    }

    // ── Mapping ───────────────────────────────────────────────
    private static TaskDto MapToDto(TaskItem t) => new()
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
        UpdatedAt = t.UpdatedAt
    };
}