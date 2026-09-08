using ndgf.Application.Commands.Refund;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.Refund;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Refund;

public class CreateRefundHandler(
  IUserRepository userRepository,
  IGroupMemberRepository groupMemberRepository,
  IRefundRepository refundRepository,
  IGroupRepository groupRepository)
{
  public async Task<Result<CreateRefundResult>> HandleAsync(CreateRefundCommand command)
  {
    bool isArchived = await groupRepository.IsArchivedAsync(command.GroupId);
    if (isArchived)
    {
      return Result<CreateRefundResult>.Failure("Ce groupe est archivé, impossible de le modifier à nouveau.");
    }
    
    bool isMember = await groupMemberRepository.IsMemberAsync(command.RequestingUserId, command.GroupId);
    if (!isMember)
    {
      return Result<CreateRefundResult>.Failure("Vous devez être membre du groupe pour creer un remboursement.");
    }

    Domain.Entities.User? payer = await userRepository.GetUserByIdAsync(command.PayerId);
    if (payer is null)
    {
      return Result<CreateRefundResult>.Failure("Membre payeur introuvable.");
    }

    Domain.Entities.User? receiver = await userRepository.GetUserByIdAsync(command.ReceiverId);
    if (receiver is null)
    {
      return Result<CreateRefundResult>.Failure("Membre receveur introuvable.");
    }

    Domain.Entities.Refund newRefund =
      Domain.Entities.Refund.Create(command.PayerId, command.ReceiverId, command.Amount, command.Description, command.GroupId);
    
    Domain.Entities.Refund savedRefund = await refundRepository.AddAsync(newRefund);

    var result = new CreateRefundResult(savedRefund, payer.Pseudo, receiver.Pseudo);
    
    return Result<CreateRefundResult>.Success(result);
  }
}