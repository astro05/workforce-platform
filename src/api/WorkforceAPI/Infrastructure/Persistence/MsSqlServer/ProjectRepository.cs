using Microsoft.EntityFrameworkCore;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Infrastructure.Persistence.MsSqlServer;

public class ProjectRepository : IProjectRepository
{
    private readonly WorkforceDbContext _ctx;

    public ProjectRepository(WorkforceDbContext ctx) => _ctx = ctx;

    public async Task<Project?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _ctx.Projects.FindAsync([id], ct);

    public async Task<Project?> GetWithDetailsAsync(int id, CancellationToken ct = default)
        => await _ctx.Projects
            .Include(p => p.ProjectEmployees).ThenInclude(pe => pe.Employee).ThenInclude(e => e.Department)
            .Include(p => p.Tasks).ThenInclude(t => t.AssignedTo)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Projects
            .Include(p => p.ProjectEmployees)
            .Include(p => p.Tasks)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Project>> GetAllWithDetailsAsync(CancellationToken ct = default)
        => await _ctx.Projects
            .Include(p => p.ProjectEmployees).ThenInclude(pe => pe.Employee)
            .Include(p => p.Tasks)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

    public async Task AddMemberAsync(int projectId, int employeeId, string role = "Member", CancellationToken ct = default)
    {
        var exists = await _ctx.ProjectEmployees.AnyAsync(pe =>
            pe.ProjectId == projectId && pe.EmployeeId == employeeId, ct);
        if (!exists)
        {
            _ctx.ProjectEmployees.Add(new ProjectEmployee
            {
                ProjectId = projectId,
                EmployeeId = employeeId,
                Role = role
            });
        }
    }

    public async Task RemoveMemberAsync(int projectId, int employeeId, CancellationToken ct = default)
    {
        var pe = await _ctx.ProjectEmployees.FindAsync([projectId, employeeId], ct);
        if (pe != null) _ctx.ProjectEmployees.Remove(pe);
    }

    public async Task<Project> AddAsync(Project entity, CancellationToken ct = default)
    {
        _ctx.Projects.Add(entity);
        return entity;
    }

    public Task UpdateAsync(Project entity, CancellationToken ct = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _ctx.Projects.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Project entity, CancellationToken ct = default)
    {
        _ctx.Projects.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _ctx.SaveChangesAsync(ct);
}
