namespace ndgf.Web.Models.Expense;

public record GeolocationResult
{
  public decimal Latitude { get; init; }
  public decimal Longitude { get; init; }
};