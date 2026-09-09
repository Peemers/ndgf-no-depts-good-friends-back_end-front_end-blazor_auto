using Microsoft.Extensions.Logging;
using ndgf.Application.Commands.Group;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Services;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Group;

public class ArchiveGroupHandler(
  IGroupMemberRepository groupMemberRepository,
  IGroupRepository groupRepository,
  IBalanceCalculator balanceCalculator,
  ILogger<ArchiveGroupHandler> logger)
{
  public async Task<Result<bool>> HandleAsync(ArchiveGroupCommand command)
  {
    bool isMember = await groupMemberRepository.IsMemberAsync(command.UserId, command.GroupId);
    if (!isMember)
    {
      logger.LogInformation("Tentative d'archivage du groupe ({GroupId}) échouée - ({UserId}) ne fait pas partie du groupe", command.GroupId, command.UserId);
      return Result<bool>.Failure("Vous devez être membre du groupe pour l'archiver");
    }

    var group = await groupRepository.GetGroupByIdAsync(command.GroupId);
    if (group is null)
    {
      logger.LogWarning("Tentative d'archivage par ({UserId}) échouée le groupe demandé n'existe pas", command.UserId);
      return Result<bool>.Failure("Groupe introuvable");
    }

    var balances = await balanceCalculator.CalculateBalanceAsync(command.GroupId);

    bool isBalanceZero = balances.All(b => b.Balance == 0);
    if (!isBalanceZero)
    {
      logger.LogInformation("Tentative d'archivage du groupe par ({UserId}) de ({GroupId}) échouée - groupe non soldé",  command.UserId, command.GroupId);
      return Result<bool>.Failure("Ce groupe n'a pas été soldé, balances pas à 0");
    }
    
    group.Archive();
    
    await groupRepository.UpdateAsync(group);
    
    logger.LogInformation("Tentative d'archivage du groupe ({GroupId}) par ({UserId}) réussie", command.GroupId, command.UserId);
    
    return Result<bool>.Success(true);
  }
}