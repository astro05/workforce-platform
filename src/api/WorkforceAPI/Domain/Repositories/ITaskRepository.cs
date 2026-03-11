using WorkforceAPI.Domain.Entities;

namespace WorkforceAPI.Domain.Repositories;

public interface ITaskRepository : IRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetByProjectAsync(int projectId, CancellationToken ct = default);
    Task<IEnumerable<TaskItem>> GetByAssigneeAsync(int employeeId, CancellationToken ct = default);
}
