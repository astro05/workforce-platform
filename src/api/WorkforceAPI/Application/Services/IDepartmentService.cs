using WorkforceAPI.Application.DTOs;

namespace WorkforceAPI.Application.Services;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync(CancellationToken ct = default);
    Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, CancellationToken ct = default);
    Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}