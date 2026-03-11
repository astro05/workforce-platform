using WorkforceAPI.Application.DTOs;

namespace WorkforceAPI.Application.Services;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeDto>> GetPagedAsync(EmployeeQueryParams query, CancellationToken ct = default);
    Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken ct = default);
    Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}