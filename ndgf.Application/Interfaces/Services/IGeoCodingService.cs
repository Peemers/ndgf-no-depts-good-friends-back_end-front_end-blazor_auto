namespace ndgf.Application.Interfaces.Services;

public interface IGeoCodingService
{
  Task<string?> GetGeoLocationAsync(decimal latitude, decimal longitude);
}