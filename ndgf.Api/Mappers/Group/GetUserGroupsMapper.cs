using ndgf.Api.Dtos.Group.Response;
using ndgf.Application.Models.Group;

namespace ndgf.Api.Mappers.Group;

public static class GetUserGroupsMapper
{
  public static GetUserGroupsResponseDto ToResponseDto(this GetUserGroupsResult result)
  {
    return new GetUserGroupsResponseDto()
    {
      Groups = result.Groups.Select(u => new UserGroupSummaryDto
      {
        Name = u.Name,
        Description = u.Description,
        Id = u.Id,
        MemberCount = u.MemberCount
      }).ToList()
    };
  }
}