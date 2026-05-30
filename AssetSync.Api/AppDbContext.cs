using Microsoft.EntityFrameworkCore;
using AssetSync.Api.Models;

namespace AssetSync.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
    {
        
    }

    public DbSet<WarehouseSale> WarehouseSales { get; set; } = null!;
}