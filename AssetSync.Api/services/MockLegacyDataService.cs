using Microsoft.EntityFrameworkCore;
using AssetSync.Api.Data;
using AssetSync.Api.Models;

namespace AssetSync.Api.Services;

public class MockLegacyDataService : ILegacyDataService
{
    private readonly AppDbContext _context;

    // inject database context
    public MockLegacyDataService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WarehouseSale>> GetLegacyDataAsync()
    {
        // Pull baseline data
        // AsNoTracking() makes it so EF Core ignores these objects - we don't want to push them
        // back into our real db
        // We limit the data set to 500 rows here so this can run a bit faster
        var legacyData = await _context.WarehouseSales.AsNoTracking().Take(500).ToListAsync();

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