using Microsoft.EntityFrameworkCore;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Infrastructure.Persistence.MsSqlServer;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly WorkforceDbContext _ctx;

    public EmployeeRepository(WorkforceDbContext ctx) => _ctx = ctx;

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _ctx.Employees
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Project)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Employees
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .ToListAsync(ct);

    public async Task<(IEnumerable<Employee> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        string? search = null,
        int? departmentId = null,
        bool? isActive = null,
        CancellationToken ct = default)
    {
        var query = _ctx.Employees
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(e =>
                e.FirstName.ToLower().Contains(s) ||
                e.LastName.ToLower().Contains(s) ||
                e.Email.ToLower().Contains(s));
        }

        if (departmentId.HasValue)
            query = query.Where(e => e.DepartmentId == departmentId.Value);

        if (isActive.HasValue)
            query = query.Where(e => e.IsActive == isActive.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(e => e.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<Employee?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _ctx.Employees.FirstOrDefaultAsync(e => e.Email == email, ct);

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default)
        => await _ctx.Employees.AnyAsync(e =>
            e.Email == email && (excludeId == null || e.Id != excludeId.Value), ct);

    public async Task<Employee> AddAsync(Employee entity, CancellationToken ct = default)
    {
        _ctx.Employees.Add(entity);
        return entity;
    }

    public Task UpdateAsync(Employee entity, CancellationToken ct = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _ctx.Employees.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Employee entity, CancellationToken ct = default)
    {
        _ctx.Employees.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _ctx.SaveChangesAsync(ct);
}
