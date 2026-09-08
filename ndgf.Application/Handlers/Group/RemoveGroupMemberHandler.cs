using ndgf.Application.Commands.Group;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Services;
using ndgf.Domain.Common;


namespace ndgf.Application.Handlers.Group;

public class RemoveGroupMemberHandler(
  IGroupMemberRepository groupMemberRepository,
  IBalanceCalculator balanceCalculator,
  IGroupRepository groupRepository
  )
{
  public async Task<Result<bool>> HandleAsync(RemoveGroupMemberCommand command)
  {
    bool isArchived = await groupRepository.IsArchivedAsync(command.GroupId);
    if (isArchived)
    {
      return Result<bool>.Failure("Ce groupe est archivé, impossible de le modifier à nouveau.");
    }
    
    bool requestingUserIsMember = await groupMemberRepository.IsMemberAsync(command.RequestingUserId, command.GroupId);
    if (!requestingUserIsMember)
    {
      return Result<bool>.Failure("Vous devez être membre du groupe pour en retirer un membre");
    }

    bool memberToRemoveIsMember = await groupMemberRepository.IsMemberAsync(command.MemberToRemoveId, command.GroupId);
    if (!memberToRemoveIsMember)
    {
      return Result<bool>.Failure("Le membre à supprimer ne faire pas partie du groupe");
    }
    
    var groupMembers = (await groupMemberRepository.GetMemberByGroupIdAsync(command.GroupId)).ToList();

    var groupCreator = groupMembers.OrderBy(gm => gm.JoinedAt).First();
    if (command.MemberToRemoveId == groupCreator.UserId && command.RequestingUserId != command.MemberToRemoveId)
    {
      return Result<bool>.Failure("Impossible de supprimer le créateur du groupe");
    }

    var balances = await balanceCalculator.CalculateBalanceAsync(command.GroupId);
    var memberBalance = balances.FirstOrDefault(ub => ub.UserId == command.MemberToRemoveId);
    if (memberBalance is not null && memberBalance.Balance != 0)
    {
      return Result<bool>.Failure("La balance du membre à supprimer doit être à 0");
    }

    if (groupMembers.Count() <= 1)
    {
      return Result<bool>.Failure("Impossible de supprimer le dernier membre du groupe, archivez plutôt le groupe");
    }
    
    var memberToRemove = groupMembers.First(gm => gm.UserId == command.MemberToRemoveId);
    await groupMemberRepository.RemoveAsync(memberToRemove);

    return Result<bool>.Success(true);
  }
}