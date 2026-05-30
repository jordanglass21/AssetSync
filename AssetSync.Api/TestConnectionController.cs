using Microsoft.AspNetCore.Mvc;
using AssetSync.Api.Data;
using System.Linq;

namespace AssetSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestConnectionController : ControllerBase
{
    private readonly AppDbContext _context;

    // inject context
    public TestConnectionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("check-data")]
    public IActionResult CheckDatabaseConnection()
    {
        try
        {
            // grab just top 5 rows from table
            var sampleData = _context.WarehouseSales.Take(5).ToList();

            // 200 OK
            return Ok(new { 
                Message = "Successfully connected to the database!", 
                TotalRowsFoundInSample = sampleData.Count,
                Data = sampleData 
            });
        }
        catch (System.Exception ex)
        {
            // 500 Error Code
            return StatusCode(500, new { 
                Message = "Database connection failed.", 
                Error = ex.Message 
            });
        }
    }
}