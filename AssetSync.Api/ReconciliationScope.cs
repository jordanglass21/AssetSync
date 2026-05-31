using AssetSync.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetSync.Api.Configuration;

public static class ReconciliationScope
{
    public static IQueryable<T> ApplyScope<T>(IQueryable<T> query, IConfiguration config) where T : WarehouseSale
    {
        if (!config.GetValue<bool>("ReconciliationSettings:UseSubset"))
            return query.OrderBy(x => x.ItemCode);

        return query
            .OrderBy(x => x.ItemCode)
            .Take(config.GetValue<int>("ReconciliationSettings:SubsetSize"));
    }
}