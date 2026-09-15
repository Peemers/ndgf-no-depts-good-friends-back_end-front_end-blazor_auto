using System.Text.Json;
using ndgf.Application.Interfaces.Services;

namespace ndgf.Infrastructure.Services;

public class GeoCodingService(HttpClient httpClient) : IGeoCodingService
{
  public async Task<string?> GetGeoLocationAsync(decimal latitude, decimal longitude)
  {
    var url = $"reverse?lat={latitude}&lon={longitude}&format=json";

    var response = await httpClient.GetAsync(url);

    if (!response.IsSuccessStatusCode)
    {
      return null;
    }
    
    var content = await response.Content.ReadAsStringAsync();
    using var document = JsonDocument.Parse(content);

    if (document.RootElement.TryGetProperty("display_name", out var displayName))
    {
      return displayName.GetString();
    }
    
    return null;
  }
}