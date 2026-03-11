using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IRepository<Department> _repo;

    public DepartmentService(IRepository<Department> repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync(CancellationToken ct = default)
    {
        var departments = await _repo.GetAllAsync(ct);
        return departments.Select(MapToDto);
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var department = await _repo.GetByIdAsync(id, ct);
        return department is null ? null : MapToDto(department);
    }

    public async Task<DepartmentDto> CreateAsync(
        CreateDepartmentDto dto, CancellationToken ct = default)
    {
        var department = new Department
        {
            Name = dto.Name,
            Description = dto.Description
        };

        await _repo.AddAsync(department, ct);
        await _repo.SaveChangesAsync(ct);

        return MapToDto(department);
    }

    public async Task<DepartmentDto> UpdateAsync(
        int id, UpdateDepartmentDto dto, CancellationToken ct = default)
    {
        var department = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Department {id} not found.");

        department.Name = dto.Name;
        department.Description = dto.Description;
        department.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(department, ct);
        await _repo.SaveChangesAsync(ct);

        return MapToDto(department);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var department = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Department {id} not found.");

        await _repo.DeleteAsync(department, ct);
        await _repo.SaveChangesAsync(ct);
    }

    // ── Mapping ───────────────────────────────────────────────
    private static DepartmentDto MapToDto(Department d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        Description = d.Description,
        EmployeeCount = d.Employees.Count,
        CreatedAt = d.CreatedAt
    };
}