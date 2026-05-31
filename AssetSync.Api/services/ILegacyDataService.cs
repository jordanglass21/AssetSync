using AssetSync.Api.Models;

namespace AssetSync.Api.Services;

public interface ILegacyDataService
{
    // contract - any class must implement this method
    Task<List<WarehouseSale>> GetLegacyDataAsync();
}