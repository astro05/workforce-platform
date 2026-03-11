using WorkforceAPI.Application.DTOs;

namespace WorkforceAPI.Application.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetByProjectAsync(int projectId, CancellationToken ct = default);
    Task<TaskDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TaskDto> CreateAsync(CreateTaskDto dto, CancellationToken ct = default);
    Task<TaskDto> UpdateAsync(int id, UpdateTaskDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}