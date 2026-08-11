using ndgf.Api.Dtos.Group.Response;
using ndgf.Application.Models.Group;

namespace ndgf.Api.Mappers.Group;

public static class GetGroupDetailsMapper
{
  public static GetGroupDetailsResponseDto ToResponseDto(this GetGroupDetailsResult result)
  {
    return new GetGroupDetailsResponseDto
    {
      Name = result.Group.Name,
      Description = result.Group.Description,
      Id = result.Group.Id,
      Members = result.Members.Select(m => new GroupMemberInfoResultDto
      {
        UserId = m.UserId,
        Pseudo = m.Pseudo,
        Email = m.Email
      }).ToList()
    };
  }
}