using Microsoft.Extensions.Logging;
using ndgf.Application.Commands.Group;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Domain.Common;
using ndgf.Domain.Entities;

namespace ndgf.Application.Handlers.Group;

public class AddGroupMemberHandler(
  IUserRepository userRepository,
  IGroupMemberRepository groupMemberRepository,
  IGroupRepository groupRepository,
  ILogger<AddGroupMemberHandler> logger)
{
  public async Task<Result<GroupMember>> HandleAsync(AddGroupMemberCommand command)
  {
    bool isArchived = await groupRepository.IsArchivedAsync(command.GroupId);
    if (isArchived)
    {
      logger.LogInformation("[Echec] Tentative d'ajouter un membre échouée - le groupe ({GroupId}) est un groupe archivé", command.GroupId);
      return Result<GroupMember>.Failure("Impossible d'ajouter un membre dans un groupe archivé.");
    }

    bool inviterIsMember = await groupMemberRepository.IsMemberAsync(command.UserId, command.GroupId);
    if (!inviterIsMember)
    {
      logger.LogInformation("[Echec] Tentative d'ajouter un membre dans le groupe ({GroupId}) échouée - ({UserId}) ne fait pas partie du groupe", command.GroupId,
        command.UserId);
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
      logger.LogInformation("[Echec] Tentative d'ajout d'un membre au groupe ({GroupId}) échouée - utilisateur introuvable pour '{SearchValue}'", command.GroupId,
        command.SearchValue);
      return Result<GroupMember>.Failure("Le membre que vous voulez ajouter au groupe n'existe pas");
    }

    bool memberAlreadyInGroup = await groupMemberRepository.IsMemberAsync(user.Id, command.GroupId);

    if (memberAlreadyInGroup)
    {
      logger.LogInformation("[Echec] Tentative d'ajouter un membre dans le groupe ({GroupId}) échouée - ({user.Id}) est deja membre du groupe", command.GroupId, user.Id);
      return Result<GroupMember>.Failure("Le membre qui vous voulez ajouter est déjà dans ce groupe");
    }

    GroupMember newUserInGroup = GroupMember.Create(user.Id, command.GroupId);

    GroupMember savedNewUserInGroup = await groupMemberRepository.AddAsync(newUserInGroup);

    logger.LogInformation("[Succès] Ajout du membre {UserId} au groupe {GroupId} réussi", user.Id, command.GroupId);

    return Result<GroupMember>.Success(savedNewUserInGroup);
  }
}