using ndgf.Api.Dtos.Refund.Request;
using ndgf.Api.Dtos.Refund.Response;
using ndgf.Application.Commands.Refund;
using ndgf.Application.Models.Refund;

namespace ndgf.Api.Mappers.Refund;

public static class CreateRefundMapper
{
  public static CreateRefundCommand ToCommand(this CreateRefundRequestDto dto, Guid requestingUserId, Guid groupId)
  {
    return new CreateRefundCommand(requestingUserId, dto.PayerId, dto.ReceiverId, dto.Amount, dto.Description, groupId);
  }

  public static CreateRefundResponseDto ToResponseDto(this CreateRefundResult result)
  {
    return new CreateRefundResponseDto
    {
      Id = result.Refund.Id,
      Amount = result.Refund.Amount,
      Description = result.Refund.Description,
      GroupId = result.Refund.GroupId,
      PayerPseudo = result.PayerPseudo,
      ReceiverPseudo = result.ReceiverPseudo,
      PayerId = result.Refund.PayerId,
      ReceiverId = result.Refund.ReceiverId
    };
  }
}