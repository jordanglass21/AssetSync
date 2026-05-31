using Microsoft.EntityFrameworkCore;
using AssetSync.Api.Data;
using AssetSync.Api.Models;
using Microsoft.Extensions.Configuration;
using AssetSync.Api.Configuration;

namespace AssetSync.Api.Services;

public class MockLegacyDataService : ILegacyDataService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    // inject database context
    public MockLegacyDataService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<List<WarehouseSale>> GetLegacyDataAsync()
    {

        var query = _context.WarehouseSales.AsNoTracking().OrderBy(x => x.ItemCode);
        var legacyData = await ReconciliationScope.ApplyScope(query, _config).ToListAsync();

        // manual corruption of data
        // Find our specific bottle - BOOTLEG RED (Item Code 100009)
        var targetItem = legacyData.FirstOrDefault(item => item.ItemCode == "100009");
        
        if (targetItem != null)
        {
            // Our database says 2.00 so we have created a mismatch
            targetItem.WarehouseSales = 50.00M;
        }

        return legacyData;
    }
}