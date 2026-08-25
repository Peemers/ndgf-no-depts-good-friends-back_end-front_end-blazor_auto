using System.ComponentModel.DataAnnotations;

namespace ndgf.Api.Dtos.Refund.Request;

public record CreateRefundRequestDto
{
  [Range(0.01, double.MaxValue)]
  public decimal Amount { get; init; }
  
  public required Guid PayerId { get; init; }
  
  public required Guid ReceiverId { get; init; }

  public string? Description { get; init; }
};