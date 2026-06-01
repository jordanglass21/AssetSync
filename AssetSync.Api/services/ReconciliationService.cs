using AssetSync.Api.Data;
using AssetSync.Api.Models;
using Microsoft.EntityFrameworkCore;
using AssetSync.Api.Configuration;

namespace AssetSync.Api.Services;

public class ReconciliationReport
{
    public int Year { get; set; }
    public int Month { get; set; }
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
    private readonly IConfiguration _config;

    public ReconciliationService(AppDbContext context, ILegacyDataService legacyDataService, IConfiguration config)
    {
        _context = context;
        _legacyDataService = legacyDataService;
        _config = config;
    }

    /* * TODO: PRODUCTION SCALING - ARCHITECTURAL UPGRADE REQUIRED
     * --------------------------------------------------------
     * Currently, we operate in 'Governance Mode' using a fixed subset size to ensure
     * rapid development and synchronization parity. 
     * * To support full-scale reconciliation of the 300k+ record dataset:
     * 1. Implement Pagination: Refactor RunReconciliationAsync to accept 'pageNumber' 
     * and 'pageSize' parameters.
     * 2. Stream Processing: Transition from loading all records into memory to 
     * processing the data as a stream to prevent OutOfMemoryExceptions.
     * 3. Async Job Scheduling: Audit triggers should return a 'jobId' and process 
     * in the background to prevent UI request timeouts.
     */
    public async Task<List<ReconciliationReport>> RunReconciliationAsync()
    {
        var discrepancies = new List<ReconciliationReport>();

        var modernData = await GetModernDataAsync();
        var legacyData = await _legacyDataService.GetLegacyDataAsync();

        foreach (var modernItem in modernData)
        {
            // Data Integrity 
            if (string.IsNullOrWhiteSpace(modernItem.ItemCode))
            {
                discrepancies.Add(CreateIntegrityFailureReport(modernItem));
                continue;
            }

            // Find the Match
            var legacyItem = legacyData.FirstOrDefault(l =>
                l.ItemCode != null &&
                l.ItemCode.Trim() == modernItem.ItemCode.Trim() &&
                l.Year == modernItem.Year &&
                l.Month == modernItem.Month);

            // Orphan Check
            if (legacyItem == null)
            {
                discrepancies.Add(CreateMissingLegacyReport(modernItem));
                continue;
            }

            // Value Comparison
            discrepancies.AddRange(CompareMetrics(modernItem, legacyItem));
        }

        return discrepancies;
    }

    // helper functions

    private ReconciliationReport CreateIntegrityFailureReport(WarehouseSale modernItem)
    {
        return new ReconciliationReport
        {
            ItemCode = "UNKNOWN",
            ItemDescription = modernItem.ItemDescription ?? "UNKNOWN",
            Metric = "Critical: Modern ItemCode is Null",
            ModernValue = modernItem.WarehouseSales,
            LegacyValue = null
        };
    }

    private ReconciliationReport CreateMissingLegacyReport(WarehouseSale modernItem)
    {
        return new ReconciliationReport
        {
            ItemCode = modernItem.ItemCode,
            ItemDescription = modernItem.ItemDescription,
            Metric = "All (Missing in Legacy)",
            ModernValue = 1,
            LegacyValue = 0
        };
    }

    private List<ReconciliationReport> CompareMetrics(WarehouseSale modernItem, WarehouseSale legacyItem)
    {
        var metricDiscrepancies = new List<ReconciliationReport>();

        // DEBUG
        // System.Console.WriteLine($"Comparing Item {modernItem.ItemCode}: Modern={modernItem.WarehouseSales}, Legacy={legacyItem.WarehouseSales}");

        if (Math.Abs((modernItem.WarehouseSales ?? 0) - (decimal)legacyItem.WarehouseSales) > 0.01M)
        {
            metricDiscrepancies.Add(BuildReport(modernItem, legacyItem, "WarehouseSales", modernItem.WarehouseSales, (decimal)legacyItem.WarehouseSales));
        }

        if (modernItem.RetailSales != legacyItem.RetailSales)
        {
            metricDiscrepancies.Add(BuildReport(modernItem, legacyItem, "RetailSales", modernItem.RetailSales, legacyItem.RetailSales));
        }

        if (modernItem.RetailTransfers != legacyItem.RetailTransfers)
        {
            metricDiscrepancies.Add(BuildReport(modernItem, legacyItem, "RetailTransfers", modernItem.RetailTransfers, legacyItem.RetailTransfers));
        }

        return metricDiscrepancies;
    }

    private ReconciliationReport BuildReport(WarehouseSale modernItem, WarehouseSale legacyItem, string metricName, decimal? modernValue, decimal? legacyValue)
    {
        return new ReconciliationReport
        {
            Year = modernItem.Year,
            Month = modernItem.Month,
            ItemCode = modernItem.ItemCode,
            ItemDescription = modernItem.ItemDescription,
            Metric = metricName,
            ModernValue = modernValue,
            LegacyValue = legacyValue
        };
    }
    private async Task<List<WarehouseSale>> GetModernDataAsync()
    {
        var query = _context.WarehouseSales.AsNoTracking().OrderBy(x => x.ItemCode);
        return await ReconciliationScope.ApplyScope(query, _config).ToListAsync();
    }
}