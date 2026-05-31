using AssetSync.Api.Data;
using AssetSync.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetSync.Api.Services;

public class ReconciliationReport
{
    public string ItemCode { get; set; } = string.Empty;
    public string ItemDescription { get; set; } = string.Empty;
    public string Metric { get; set; } = string.Empty;
    public decimal? ModernValue { get; set; }
    public decimal? LegacyValue { get; set; }
    public decimal Discrepancy => (LegacyValue ?? 0M) - (ModernValue ?? 0M);
}

public class ReconciliationService
{
    private readonly AppDbContext _context;
    private readonly ILegacyDataService _legacyDataService;

    // We inject both modern database context and the legacy context
    public ReconciliationService(AppDbContext context, ILegacyDataService legacyDataService)
    {
        _context = context;
        _legacyDataService = legacyDataService;
    }

    public async Task<List<ReconciliationReport>> RunReconciliationAsync()
    {
        var discrepancies = new List<ReconciliationReport>();

        // fetch data from both contexts
        // we can use await to do this in parallel
        // We limit the data set to 500 rows here so this can run a bit faster
        var modernData = await _context.WarehouseSales.Take(500).ToListAsync();
        var legacyData = await _legacyDataService.GetLegacyDataAsync();

        // loop through and compare rows based on the composite key
        foreach (var modernItem in modernData)
        {
            // Find the matching item in the legacy dataset
            var legacyItem = legacyData.FirstOrDefault(l => 
                l.Year == modernItem.Year && 
                l.Month == modernItem.Month && 
                l.ItemCode == modernItem.ItemCode);

            if (legacyItem == null)
            {
                // if the row is missing entirely from the legacy archive
                discrepancies.Add(new ReconciliationReport
                {
                    ItemCode = modernItem.ItemCode,
                    ItemDescription = modernItem.ItemDescription,
                    Metric = "All (Missing in Legacy)",
                    ModernValue = 1,
                    LegacyValue = 0
                });
                continue;
            }

            // if the rows do not match
            if (modernItem.WarehouseSales != legacyItem.WarehouseSales)
            {
                discrepancies.Add(new ReconciliationReport
                {
                    ItemCode = modernItem.ItemCode,
                    ItemDescription = modernItem.ItemDescription,
                    Metric = "WarehouseSales",
                    ModernValue = modernItem.WarehouseSales,
                    LegacyValue = legacyItem.WarehouseSales
                });
            }

            if (modernItem.RetailSales != legacyItem.RetailSales)
            {
                discrepancies.Add(new ReconciliationReport
                {
                    ItemCode = modernItem.ItemCode,
                    ItemDescription = modernItem.ItemDescription,
                    Metric = "RetailSales",
                    ModernValue = modernItem.RetailSales,
                    LegacyValue = legacyItem.RetailSales
                });
            }

            if (modernItem.RetailTransfers != legacyItem.RetailTransfers)
            {
                discrepancies.Add(new ReconciliationReport
                {
                    ItemCode = modernItem.ItemCode,
                    ItemDescription = modernItem.ItemDescription,
                    Metric = "RetailTransfers",
                    ModernValue = modernItem.RetailTransfers,
                    LegacyValue = legacyItem.RetailTransfers
                });
            }
        }

        return discrepancies;
    }
}