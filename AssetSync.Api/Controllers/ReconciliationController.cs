using Microsoft.AspNetCore.Mvc;
using AssetSync.Api.Services;

namespace AssetSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReconciliationController : ControllerBase
{
    private readonly ReconciliationService _reconciliationService;

    // Inject reconciliation brain
    public ReconciliationController(ReconciliationService reconciliationService)
    {
        _reconciliationService = reconciliationService;
    }

    [HttpGet("run-audit")]
    public async Task<IActionResult> RunAudit()
    {
        try
        {
            // Execute the comparison logic
            var report = await _reconciliationService.RunReconciliationAsync();
            
            return Ok(new
            {
                status = "Success",
                totalDiscrepanciesFound = report.Count,
                auditTimestamp = DateTime.UtcNow,
                discrepancies = report
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Audit engine failed", error = ex.Message });
        }
    }
}