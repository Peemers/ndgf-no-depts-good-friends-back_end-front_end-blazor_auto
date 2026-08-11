using ndgf.Application.Commands.Group;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Domain.Common;
using ndgf.Domain.Entities;

namespace ndgf.Application.Handlers.Group;

public class AddGroupMemberHandler(
  IUserRepository userRepository,
  IGroupMemberRepository groupMemberRepository)
{
  public async Task<Result<GroupMember>> HandleAsync(AddGroupMemberCommand command)
  {
    bool inviterIsMember = await groupMemberRepository.IsMemberAsync(command.UserId, command.GroupId);
    if (!inviterIsMember)
    {
      return Result<GroupMember>.Failure("Vous devez être membre du groupe en question pour inviter d'autres membres");
    }

    Domain.Entities.User? user;
    if (command.SearchValue.Contains("@"))
    {
      user = await userRepository.GetUserByEmailAsync(command.SearchValue);
    }
    else
    {
      user = await userRepository.GetUserByPseudoAsync(command.SearchValue);
    }

    if (user is null)
    {
      return Result<GroupMember>.Failure("Le membre que vous voulez ajouter au groupe n'existe pas");
    }

    bool memberAlreadyInGroup = await groupMemberRepository.IsMemberAsync(user.Id, command.GroupId);

    if (memberAlreadyInGroup)
    {
      return Result<GroupMember>.Failure("Le membre qui vous voulez ajouter est déjà dans ce groupe");
    }

    GroupMember newUserInGroup = GroupMember.Create(user.Id, command.GroupId);
    
    GroupMember savedNewUserInGroup = await groupMemberRepository.AddAsync(newUserInGroup);
    return Result<GroupMember>.Success(savedNewUserInGroup);
  }
}