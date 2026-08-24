namespace ndgf.Api.Dtos.Refund.Response;

public record CreateRefundResponseDto
{
  public required Guid Id { get; init; }
  public required Guid PayerId { get; init; }
  public required Guid ReceiverId { get; init; }
  public required Guid GroupId { get; init; }
  public decimal Amount { get; init; }
  public string? Description { get; init; }
  public required string PayerPseudo { get; init; }
  public required string ReceiverPseudo { get; init; }
};