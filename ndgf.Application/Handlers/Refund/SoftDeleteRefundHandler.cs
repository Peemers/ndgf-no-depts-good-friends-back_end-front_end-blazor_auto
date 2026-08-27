using ndgf.Application.Commands.Refund;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Refund;

public class SoftDeleteRefundHandler(
  IRefundRepository refundRepository,
  IGroupMemberRepository groupMemberRepository)
{
  public async Task<Result<bool>> HandleAsync(SoftDeleteRefundCommand command)
  {
    bool isMember = await groupMemberRepository.IsMemberAsync(command.UserId, command.GroupId);
    if (!isMember)
    {
      return Result<bool>.Failure("Vous devez être membre du groupe pour supprimer un remboursement");
    }

    var refund = await refundRepository.GetRefundByIdAsync(command.RefundId);
    if (refund is null)
    {
      return Result<bool>.Failure("Remboursement introuvable");
    }
    
    refund.SoftDelete();
    
    await refundRepository.UpdateAsync(refund);
    
    return Result<bool>.Success(true);
  }
}