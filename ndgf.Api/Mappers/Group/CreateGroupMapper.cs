using ndgf.Api.Dtos.Group.Request;
using ndgf.Api.Dtos.Group.Response;
using ndgf.Application.Commands.Group;
using ndgf.Application.Models.Group;

namespace ndgf.Api.Mappers.Group;

public static class CreateGroupMapper
{
  public static CreateGroupCommand ToCommand(this CreateGroupRequestDto dto, Guid id)
  {
    return new CreateGroupCommand(dto.Name, dto.Description, id);
  }

  public static CreateGroupResponseDto ToResponseDto(this CreateGroupResult result)
  {
    return new CreateGroupResponseDto
    {
      Id = result.Group.Id,
      Name = result.Group.Name,
      Description = result.Group.Description
    };
  }
}