using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.Group;
using ndgf.Application.Queries.Group;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Group;

public class GetGroupDetailsHandler(
  IGroupRepository groupRepository,
  IGroupMemberRepository groupMemberRepository,
  IUserRepository userRepository)
{
  public async Task<Result<GetGroupDetailsResult>> HandleAsync(GetGroupDetailQuery query)
  {
    var group = await groupRepository.GetGroupByIdAsync(query.GroupId);
    if (group == null)
    {
      return Result<GetGroupDetailsResult>.Failure("Ce groupe n'existe pas");
    }

    bool isMember = await groupMemberRepository.IsMemberAsync(query.UserId, query.GroupId);
    if (!isMember)
    {
      return Result<GetGroupDetailsResult>.Failure("Vous devez etre membre du groupe pour en consulter les détails");
    }

    var groupMembers = await groupMemberRepository.GetMemberByGroupIdAsync(query.GroupId);


    var membersInfo = new List<GroupMemberInfoResult>();

    foreach (var groupMember in groupMembers)
    {
      var user = await userRepository.GetUserByIdAsync(groupMember.UserId);

      if (user is not null)
      {
        membersInfo.Add(new GroupMemberInfoResult(user.Id, user.Pseudo, user.Email));
      }
    }

    var result = new GetGroupDetailsResult(group, membersInfo);

    return Result<GetGroupDetailsResult>.Success(result);
  }
}