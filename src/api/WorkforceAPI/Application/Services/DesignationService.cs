using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Domain.Entities;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Application.Services;

public class DesignationService : IDesignationService
{
    private readonly IRepository<Designation> _repo;

    public DesignationService(IRepository<Designation> repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<DesignationDto>> GetAllAsync(CancellationToken ct = default)
    {
        var designations = await _repo.GetAllAsync(ct);
        return designations.Select(MapToDto);
    }

    public async Task<DesignationDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var designation = await _repo.GetByIdAsync(id, ct);
        return designation is null ? null : MapToDto(designation);
    }

    public async Task<DesignationDto> CreateAsync(
        CreateDesignationDto dto, CancellationToken ct = default)
    {
        var designation = new Designation
        {
            Name = dto.Name,
            Level = dto.Level
        };

        await _repo.AddAsync(designation, ct);
        await _repo.SaveChangesAsync(ct);

        return MapToDto(designation);
    }

    public async Task<DesignationDto> UpdateAsync(
        int id, UpdateDesignationDto dto, CancellationToken ct = default)
    {
        var designation = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Designation {id} not found.");

        designation.Name = dto.Name;
        designation.Level = dto.Level;
        designation.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(designation, ct);
        await _repo.SaveChangesAsync(ct);

        return MapToDto(designation);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var designation = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Designation {id} not found.");

        await _repo.DeleteAsync(designation, ct);
        await _repo.SaveChangesAsync(ct);
    }

    // ── Mapping ───────────────────────────────────────────────
    private static DesignationDto MapToDto(Designation d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        Level = d.Level,
        EmployeeCount = d.Employees.Count,
        CreatedAt = d.CreatedAt
    };
}