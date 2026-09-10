using Microsoft.Extensions.Logging;
using ndgf.Application.Commands.Expense;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.Expense;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Expense;

public class CreateExpenseHandler(
  IUserRepository userRepository,
  IExpenseRepository expenseRepository,
  IGroupMemberRepository groupMemberRepository,
  IGroupRepository groupRepository,
  ILogger<CreateExpenseHandler> logger)
{
  public async Task<Result<CreateExpenseResult>> HandleAsync(CreateExpenseCommand command)
  {
    bool isArchived = await groupRepository.IsArchivedAsync(command.GroupId);
    if (isArchived)
    {
      logger.LogInformation("[Echec] Tentative de création d'une dépense dans le groupe ({GroupId}) par ({RequestingUserId}) - groupe archivé", command.GroupId,
        command.RequestingUserId);
      return Result<CreateExpenseResult>.Failure("Ce groupe est archivé, impossible de le modifier à nouveau.");
    }

    bool isMember = await groupMemberRepository.IsMemberAsync(command.RequestingUserId, command.GroupId);
    if (!isMember)
    {
      logger.LogInformation("[Echec] Tentative de création d'une dépense dans le groupe ({GroupId}) par ({RequestingUserId}) car il ne fait pas partie du groupe)",
        command.GroupId, command.RequestingUserId);
      return Result<CreateExpenseResult>.Failure("Vous devez être membre du groupe pour creer une dépense.");
    }

    Domain.Entities.User? payer = await userRepository.GetUserByIdAsync(command.PayerId);
    if (payer is null)
    {
      logger.LogInformation("[Echec] Tentative de création d'une dépense dans le groupe ({GroupId}) par ({RequestingUserId}) échouée - le membre payeur n'existe pas.",
        command.GroupId, command.RequestingUserId);
      return Result<CreateExpenseResult>.Failure("Membre introuvable");
    }

    Domain.Entities.Expense newExpense =
      Domain.Entities.Expense.Create(command.PayerId, command.ExpensePartInputs, command.Amount, command.Description, command.GroupId);

    Domain.Entities.Expense savedExpense = await expenseRepository.AddAsync(newExpense);

    var amountsByUser = savedExpense.CalculateAmountsByUser();
    var userExpenseInfos = new List<UserExpenseInfoResult>();

    foreach (var part in savedExpense.ExpenseParts)
    {
      var user = await userRepository.GetUserByIdAsync(part.UserId);
      if (user is not null)
      {
        var amount = amountsByUser[part.UserId];
        userExpenseInfos.Add(new UserExpenseInfoResult(user.Id, user.Pseudo, part.Percentage, amount, user.Email));
      }
    }

    var result = new CreateExpenseResult(savedExpense, userExpenseInfos, payer.Pseudo, payer.Email);

    logger.LogInformation("[Succés] Création d'un dépense réussie dans le groupe ({GroupId}) par ({RequestingUserId}) réussie - payeur : ({PayerId})", command.GroupId,
      command.RequestingUserId, command.PayerId);

    return Result<CreateExpenseResult>.Success(result);
  }
}