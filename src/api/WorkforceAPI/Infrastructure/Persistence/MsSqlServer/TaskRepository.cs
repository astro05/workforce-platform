using Microsoft.EntityFrameworkCore;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Infrastructure.Persistence.MsSqlServer;

public class TaskRepository : ITaskRepository
{
    private readonly WorkforceDbContext _ctx;

    public TaskRepository(WorkforceDbContext ctx) => _ctx = ctx;

    public async Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _ctx.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IEnumerable<TaskItem>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Tasks.Include(t => t.AssignedTo).ToListAsync(ct);

    public async Task<IEnumerable<TaskItem>> GetByProjectAsync(int projectId, CancellationToken ct = default)
        => await _ctx.Tasks
            .Include(t => t.AssignedTo)
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Priority)
            .ToListAsync(ct);

    public async Task<IEnumerable<TaskItem>> GetByAssigneeAsync(int employeeId, CancellationToken ct = default)
        => await _ctx.Tasks
            .Include(t => t.Project)
            .Where(t => t.AssignedToId == employeeId)
            .ToListAsync(ct);

    public async Task<TaskItem> AddAsync(TaskItem entity, CancellationToken ct = default)
    {
        _ctx.Tasks.Add(entity);
        return entity;
    }

    public Task UpdateAsync(TaskItem entity, CancellationToken ct = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _ctx.Tasks.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TaskItem entity, CancellationToken ct = default)
    {
        _ctx.Tasks.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _ctx.SaveChangesAsync(ct);
}
