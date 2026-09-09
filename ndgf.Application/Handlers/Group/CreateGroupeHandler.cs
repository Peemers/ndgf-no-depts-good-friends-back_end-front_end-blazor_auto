using Microsoft.Extensions.Logging;
using ndgf.Application.Commands.Group;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.Group;
using ndgf.Domain.Common;
using ndgf.Domain.Entities;

namespace ndgf.Application.Handlers.Group;

public class CreateGroupeHandler(
  IGroupRepository groupRepository,
  IGroupMemberRepository groupMemberRepository,
  ILogger<CreateGroupeHandler> logger)
{
  public async Task<Result<CreateGroupResult>> HandleAsync(CreateGroupCommand command)
  {
    Domain.Entities.Group group = Domain.Entities.Group.Create(command.Name, command.Description);

    Domain.Entities.Group savedGroup = await groupRepository.AddAsync(group);
    
    GroupMember groupMember = GroupMember.Create(command.UserId, savedGroup.Id);

    GroupMember savedGroupMember = await groupMemberRepository.AddAsync(groupMember);
    
    var result = new CreateGroupResult(savedGroup, savedGroupMember);
    
    logger.LogInformation("Tentative de création d'un nouveau groupe ({Name}) reussie par ({UserId}) ", command.Name, command.UserId);
    
    return Result<CreateGroupResult>.Success(result);
  }
}