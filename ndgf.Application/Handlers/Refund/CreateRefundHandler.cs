using Microsoft.Extensions.Logging;
using ndgf.Application.Commands.Refund;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.Refund;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Refund;

public class CreateRefundHandler(
  IUserRepository userRepository,
  IGroupMemberRepository groupMemberRepository,
  IRefundRepository refundRepository,
  IGroupRepository groupRepository,
  ILogger<CreateRefundHandler> logger)
{
  public async Task<Result<CreateRefundResult>> HandleAsync(CreateRefundCommand command)
  {
    bool isArchived = await groupRepository.IsArchivedAsync(command.GroupId);
    if (isArchived)
    {
      logger.LogInformation("[Echec] Tentative de création d'un remboursement dans le groupe ({GroupId}) par ({RequestingUserId}) échouée - groupe archivé",
        command.GroupId, command.RequestingUserId);
      return Result<CreateRefundResult>.Failure("Ce groupe est archivé, impossible de le modifier à nouveau.");
    }

    bool isMember = await groupMemberRepository.IsMemberAsync(command.RequestingUserId, command.GroupId);
    if (!isMember)
    {
      logger.LogInformation("[Echec] Tentative de création d'un remboursement dans le groupe ({GroupId}) échouée - ({RequestingUserId}) n'est pas membre du groupe",
        command.GroupId, command.RequestingUserId);
      return Result<CreateRefundResult>.Failure("Vous devez être membre du groupe pour creer un remboursement.");
    }

    Domain.Entities.User? payer = await userRepository.GetUserByIdAsync(command.PayerId);
    if (payer is null)
    {
      logger.LogInformation(
        "[Echec] Tentative de création d'un remboursement dans le groupe ({GroupId}) par ({RequestingUserId}) échouée - membre payeur de ce remboursement inéxistant",
        command.GroupId, command.RequestingUserId);
      return Result<CreateRefundResult>.Failure("Membre payeur introuvable.");
    }

    Domain.Entities.User? receiver = await userRepository.GetUserByIdAsync(command.ReceiverId);
    if (receiver is null)
    {
      logger.LogInformation(
        "[Echec] Tentative de création d'un remboursement dans le groupe ({GroupId}) par ({RequestingUserId}) échouée - membre receveur de ce remboursement inexistant",
        command.GroupId, command.RequestingUserId);
      return Result<CreateRefundResult>.Failure("Membre receveur introuvable.");
    }

    Domain.Entities.Refund newRefund =
      Domain.Entities.Refund.Create(command.PayerId, command.ReceiverId, command.Amount, command.Description, command.GroupId);

    Domain.Entities.Refund savedRefund = await refundRepository.AddAsync(newRefund);

    var result = new CreateRefundResult(savedRefund, payer.Pseudo, receiver.Pseudo);

    logger.LogInformation(
      "[Succès] Tentative de création d'une dépense dans le groupe ({GroupId}) par ({RequestingUserId}) avec payeur ({PayerId}) et receveur ({ReceiverId}) reussie",
      command.GroupId, command.RequestingUserId, command.PayerId, command.ReceiverId);

    return Result<CreateRefundResult>.Success(result);
  }
}