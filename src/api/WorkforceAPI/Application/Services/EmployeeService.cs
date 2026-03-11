using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Events;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repo;

    public EmployeeService(IEmployeeRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResult<EmployeeDto>> GetPagedAsync(
        EmployeeQueryParams query, CancellationToken ct = default)
    {
        var (items, total) = await _repo.GetPagedAsync(
            query.Page, query.PageSize,
            query.Search, query.DepartmentId,
            query.IsActive, ct);

        return new PagedResult<EmployeeDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = total,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var employee = await _repo.GetByIdAsync(id, ct);
        return employee is null ? null : MapToDto(employee);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken ct = default)
    {
        if (await _repo.EmailExistsAsync(dto.Email, null, ct))
            throw new InvalidOperationException($"Email '{dto.Email}' is already in use.");

        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Salary = dto.Salary,
            JoiningDate = dto.JoiningDate,
            DepartmentId = dto.DepartmentId,
            DesignationId = dto.DesignationId,
            Phone = dto.Phone,
            Address = dto.Address,
            City = dto.City,
            Country = dto.Country,
            AvatarUrl = dto.AvatarUrl,
            Skills = dto.Skills,
            IsActive = true
        };

        await _repo.AddAsync(employee, ct);
        await _repo.SaveChangesAsync(ct);

        return MapToDto(employee);
    }

    public async Task<EmployeeDto> UpdateAsync(
        int id, UpdateEmployeeDto dto, CancellationToken ct = default)
    {
        var employee = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Employee {id} not found.");

        if (await _repo.EmailExistsAsync(dto.Email, id, ct))
            throw new InvalidOperationException($"Email '{dto.Email}' is already in use.");

        // Capture before snapshot for audit
        var before = new
        {
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.IsActive,
            employee.DepartmentId,
            employee.DesignationId
        };

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.Salary = dto.Salary;
        employee.IsActive = dto.IsActive;
        employee.DepartmentId = dto.DepartmentId;
        employee.DesignationId = dto.DesignationId;
        employee.Phone = dto.Phone;
        employee.Address = dto.Address;
        employee.City = dto.City;
        employee.Country = dto.Country;
        employee.AvatarUrl = dto.AvatarUrl;
        employee.Skills = dto.Skills;
        employee.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(employee, ct);
        await _repo.SaveChangesAsync(ct);

        return MapToDto(employee);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var employee = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Employee {id} not found.");

        // Soft delete
        employee.IsActive = false;
        employee.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(employee, ct);
        await _repo.SaveChangesAsync(ct);
    }

    // ── Mapping ───────────────────────────────────────────────
    private static EmployeeDto MapToDto(Employee e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        FullName = e.FullName,
        Email = e.Email,
        IsActive = e.IsActive,
        Salary = e.Salary,
        JoiningDate = e.JoiningDate,
        Phone = e.Phone,
        Address = e.Address,
        City = e.City,
        Country = e.Country,
        AvatarUrl = e.AvatarUrl,
        Skills = e.Skills,
        DepartmentId = e.DepartmentId,
        DepartmentName = e.Department?.Name ?? string.Empty,
        DesignationId = e.DesignationId,
        DesignationName = e.Designation?.Name ?? string.Empty,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}