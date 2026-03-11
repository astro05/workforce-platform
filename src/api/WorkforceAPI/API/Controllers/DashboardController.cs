using Microsoft.AspNetCore.Mvc;
using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Application.Services;

namespace WorkforceAPI.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IDashboardService service,
        ILogger<DashboardController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // ── GET /api/v1/dashboard ─────────────────────────────────
    [HttpGet]
    [ProducesResponseType(typeof(DashboardReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReport(CancellationToken ct)
    {
        var report = await _service.GetReportAsync(ct);

        if (report is null)
        {
            _logger.LogWarning(
                "Dashboard report not yet generated");

            return NotFound(new
            {
                message = "Dashboard report not yet available. " +
                          "The report worker generates it every 5 minutes."
            });
        }

        return Ok(report);
    }
}