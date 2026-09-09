using Microsoft.Extensions.Logging;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.Group;
using ndgf.Application.Queries.Group;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Group;

public class GetUserGroupsHandler(
  IGroupMemberRepository groupMemberRepository,
  IGroupRepository groupRepository,
  ILogger<GetUserGroupsHandler> logger)
{
  public async Task<Result<GetUserGroupsResult>> HandleAsync(GetUserGroupsQuery query)
  {
    var memberShips = await groupMemberRepository.GetMembershipsByUserIdAsync(query.UserId);

    var groups = new List<UserGroupSummaryResult>();

    foreach (var membership in memberShips)
    {
      
      var group = await groupRepository.GetGroupByIdAsync(membership.GroupId);

      if (group is not null)
      {
        var groupMembers = await groupMemberRepository.GetMemberByGroupIdAsync(group.Id);
        var memberCount = groupMembers.Count();

        groups.Add(new UserGroupSummaryResult(group.Id, group.Name, group.Description, memberCount, group.ArchivedAt));
      }
    }

    var result = new GetUserGroupsResult(groups);
    
    logger.LogInformation("Tentative de récupération des groupes du membre ({UserId}) réussie", query.UserId);

    return Result<GetUserGroupsResult>.Success(result);
  }
}