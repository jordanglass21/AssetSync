using Microsoft.EntityFrameworkCore;
using AssetSync.Api.Models;

namespace AssetSync.Api.Data;

// we inheret from DbCOntext from EF Core
public class AppDbContext : DbContext
{
    // register the context in Program.cs
    public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
    {
        
    }

    // WarehouseSale is a single row
    // this represents a table made up of rows
    public DbSet<WarehouseSale> WarehouseSales { get; set; } = null!;
}