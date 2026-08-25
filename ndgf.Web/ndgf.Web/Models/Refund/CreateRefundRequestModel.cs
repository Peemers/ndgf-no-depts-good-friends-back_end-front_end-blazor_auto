using System.ComponentModel.DataAnnotations;

namespace ndgf.Web.Models.Refund;

public record CreateRefundRequestModel
{
  [Range(0.01, double.MaxValue)]
  public decimal Amount { get; set; }
  
  public required Guid PayerId { get; set; }
  
  public required Guid ReceiverId { get; set; }

  public string? Description { get; set; }
};