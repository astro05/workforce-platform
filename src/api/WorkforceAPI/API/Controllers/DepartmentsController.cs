using Microsoft.AspNetCore.Mvc;
using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Application.Services;

namespace WorkforceAPI.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _service;
    private readonly ILogger<DepartmentsController> _logger;

    public DepartmentsController(
        IDepartmentService service,
        ILogger<DepartmentsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // ── GET /api/v1/departments ───────────────────────────────
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DepartmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var departments = await _service.GetAllAsync(ct);
        return Ok(departments);
    }

    // ── GET /api/v1/departments/{id} ──────────────────────────
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var department = await _service.GetByIdAsync(id, ct);

        if (department is null)
        {
            _logger.LogWarning("Department {Id} not found", id);
            return NotFound(new { message = $"Department {id} not found." });
        }

        return Ok(department);
    }

    // ── POST /api/v1/departments ──────────────────────────────
    [HttpPost]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateDepartmentDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateAsync(dto, ct);
        _logger.LogInformation(
            "Department created: {Id} - {Name}", created.Id, created.Name);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    // ── PUT /api/v1/departments/{id} ──────────────────────────
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateDepartmentDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _service.UpdateAsync(id, dto, ct);
            _logger.LogInformation(
                "Department updated: {Id} - {Name}", updated.Id, updated.Name);

            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // ── DELETE /api/v1/departments/{id} ───────────────────────
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _service.DeleteAsync(id, ct);
            _logger.LogInformation("Department deleted: {Id}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}