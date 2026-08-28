using ndgf.Application.Commands.Group;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Services;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Group;

public class ArchiveGroupHandler(
  IGroupMemberRepository groupMemberRepository,
  IGroupRepository groupRepository,
  IBalanceCalculator balanceCalculator)
{
  public async Task<Result<bool>> HandleAsync(ArchiveGroupCommand command)
  {
    bool isMember = await groupMemberRepository.IsMemberAsync(command.UserId, command.GroupId);
    if (!isMember)
    {
      return Result<bool>.Failure("Vous devez être membre du groupe pour l'archiver");
    }

    var group = await groupRepository.GetGroupByIdAsync(command.GroupId);
    if (group is null)
    {
      return Result<bool>.Failure("Groupe introuvable");
    }

    var balances = await balanceCalculator.CalculateBalanceAsync(command.GroupId);

    bool isBalanceZero = balances.All(b => b.Balance == 0);
    if (!isBalanceZero)
    {
      return Result<bool>.Failure("Ce groupe n'a pas été soldé, balances pas à 0");
    }
    
    group.Archive();
    
    await groupRepository.UpdateAsync(group);
    
    return Result<bool>.Success(true);
  }
}