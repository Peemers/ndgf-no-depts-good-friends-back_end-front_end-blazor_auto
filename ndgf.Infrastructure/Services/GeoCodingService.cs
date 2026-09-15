using System.Globalization;
using System.Text.Json;
using ndgf.Application.Interfaces.Services;

namespace ndgf.Infrastructure.Services;

public class GeoCodingService(HttpClient httpClient) : IGeoCodingService
{
  public async Task<string?> GetGeoLocationAsync(decimal latitude, decimal longitude)
  {
    var url = $"reverse?lat={latitude.ToString(CultureInfo.InvariantCulture)}&lon={longitude.ToString(CultureInfo.InvariantCulture)}&format=json&accept-language=fr";
    
    var response = await httpClient.GetAsync(url);

    if (!response.IsSuccessStatusCode)
    {
      return null;
    }
    
    var content = await response.Content.ReadAsStringAsync();
    
    using var document = JsonDocument.Parse(content);

    if (document.RootElement.TryGetProperty("address", out var address))
    {
      var city = GetAddressField(address, "town") ?? GetAddressField(address, "city") ?? GetAddressField(address, "village");
      var state = GetAddressField(address, "state");
      var postcode = GetAddressField(address, "postcode");
      var country = GetAddressField(address, "country");

      var parts = new[] { city, postcode, state, country }.Where(p => !string.IsNullOrWhiteSpace(p));
      return string.Join(", ", parts);
    }
    
    return null;
  }
  private static string? GetAddressField(JsonElement address, string field)
  {
    return address.TryGetProperty(field, out var value) ? value.GetString() : null;
  }
}

