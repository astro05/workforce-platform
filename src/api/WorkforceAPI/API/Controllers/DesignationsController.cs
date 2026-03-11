using Microsoft.AspNetCore.Mvc;
using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Application.Services;

namespace WorkforceAPI.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DesignationsController : ControllerBase
{
    private readonly IDesignationService _service;
    private readonly ILogger<DesignationsController> _logger;

    public DesignationsController(
        IDesignationService service,
        ILogger<DesignationsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // ── GET /api/v1/designations ──────────────────────────────
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DesignationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var designations = await _service.GetAllAsync(ct);
        return Ok(designations);
    }

    // ── GET /api/v1/designations/{id} ─────────────────────────
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DesignationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var designation = await _service.GetByIdAsync(id, ct);

        if (designation is null)
        {
            _logger.LogWarning("Designation {Id} not found", id);
            return NotFound(new { message = $"Designation {id} not found." });
        }

        return Ok(designation);
    }

    // ── POST /api/v1/designations ─────────────────────────────
    [HttpPost]
    [ProducesResponseType(typeof(DesignationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateDesignationDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateAsync(dto, ct);
        _logger.LogInformation(
            "Designation created: {Id} - {Name}", created.Id, created.Name);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    // ── PUT /api/v1/designations/{id} ─────────────────────────
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(DesignationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateDesignationDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _service.UpdateAsync(id, dto, ct);
            _logger.LogInformation(
                "Designation updated: {Id} - {Name}", updated.Id, updated.Name);

            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // ── DELETE /api/v1/designations/{id} ──────────────────────
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _service.DeleteAsync(id, ct);
            _logger.LogInformation("Designation deleted: {Id}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}