using Microsoft.Extensions.Logging;
using ndgf.Application.Commands.Refund;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Refund;

public class SoftDeleteRefundHandler(
  IRefundRepository refundRepository,
  IGroupMemberRepository groupMemberRepository,
  IGroupRepository groupRepository,
  ILogger<SoftDeleteRefundHandler> logger)
{
  public async Task<Result<bool>> HandleAsync(SoftDeleteRefundCommand command)
  {
    bool isArchived = await groupRepository.IsArchivedAsync(command.GroupId);
    if (isArchived)
    {
      logger.LogInformation("[Echec] Tentative de création d'un remboursement dans le groupe ({GroupId}) par ({UserId}) échouée - groupe archivé",
        command.GroupId, command.UserId);
      return Result<bool>.Failure("Ce groupe est archivé, impossible de le modifier à nouveau.");
    }

    bool isMember = await groupMemberRepository.IsMemberAsync(command.UserId, command.GroupId);
    if (!isMember)
    {
      logger.LogInformation("[Echec] Tentative de suppression d'un remboursement dans le groupe ({GroupId}) échouée - ({UserId}) ne fait pas partie de ce groupe",
        command.GroupId, command.UserId);
      return Result<bool>.Failure("Vous devez être membre du groupe pour supprimer un remboursement");
    }

    var refund = await refundRepository.GetRefundByIdAsync(command.RefundId);
    if (refund is null)
    {
      logger.LogInformation("[Echec] Tentative de suppression d'un remboursement dans le groupe ({GroupId}) par ({UserId}) échouée - Remboursement inexistant",
        command.GroupId, command.UserId);
      return Result<bool>.Failure("Remboursement introuvable");
    }

    if (refund.DeletedAt is not null)
    {
      logger.LogInformation(
        "[Echec] Tentative de suppression d'un remboursement dans le groupe ({GroupId}) par ({UserId}) échouée - ({RefundId}) déja supprimée le ({DeletedAt})",
        command.GroupId, command.UserId, command.RefundId, refund.DeletedAt);
      return Result<bool>.Failure("Ce remboursement est déjà supprimé");
    }

    refund.SoftDelete();

    await refundRepository.UpdateAsync(refund);

    logger.LogInformation("[Succès] Tentative de suppression d'un remboursement de le groupe ({GroupId}) par ({UserId}) réussie", command.GroupId, command.UserId);

    return Result<bool>.Success(true);
  }
}