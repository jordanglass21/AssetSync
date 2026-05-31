using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AssetSync.Api.Models;

[Table("Warehouse_and_Retail_Sales")] // maps to SQLite data name
[PrimaryKey(nameof(Year), nameof(Month), nameof(ItemCode))] // combines three columns to form unique id

public class WarehouseSale
{
    [Column("YEAR")] // maps to columns in the table
    public int Year { get; set; }

    [Column("MONTH")]
    public int Month { get; set; }

    [Column("SUPPLIER")]
    public string? Supplier { get; set; } // ? means allowed to be empty or null

    [Column("ITEM CODE")]
    public string? ItemCode { get; set; }

    [Column("ITEM DESCRIPTION")]
    public string? ItemDescription { get; set; }

    [Column("ITEM TYPE")]
    public string? ItemType { get; set; }

    [Column("RETAIL SALES")]
    public decimal? RetailSales { get; set; }

    [Column("RETAIL TRANSFERS")]
    public decimal? RetailTransfers { get; set; }

    [Column("WAREHOUSE SALES")]
    public decimal? WarehouseSales { get; set; }
}