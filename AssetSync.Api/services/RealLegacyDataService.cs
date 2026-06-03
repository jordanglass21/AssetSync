using System.Text.Json;
using AssetSync.Api.Models;

namespace AssetSync.Api.Services;

public class RealLegacyDataService : ILegacyDataService 
{
    private readonly HttpClient _httpClient;

    public RealLegacyDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // TODO: Add HttpClient timeout and cancellation token to legacy data fetch.
    // Currently hangs indefinitely if the legacy service is unreachable.
    // Consider a circuit breaker pattern for production resilience.
    public async Task<List<WarehouseSale>> GetLegacyDataAsync()
    {
        // Call to the Java Server
        var response = await _httpClient.GetAsync("http://localhost:8080/api/legacy/sales");
        
        response.EnsureSuccessStatusCode(); 

        // read response
        var json = await response.Content.ReadAsStringAsync();
        // Console.WriteLine($"\n--- DEBUG: RAW JAVA PAYLOAD ---\n{json}\n-------------------------------\n");
        
        var options = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        };

        // convert JSON directly to C# model
        var data = JsonSerializer.Deserialize<List<WarehouseSale>>(json, options);

        return data ?? new List<WarehouseSale>();
    }
}