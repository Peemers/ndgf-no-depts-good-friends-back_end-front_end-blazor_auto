using ndgf.Application.Commands.Expense;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.Expense;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Expense;

public class CreateExpenseHandler(
  IUserRepository userRepository,
  IExpenseRepository expenseRepository,
  IGroupMemberRepository groupMemberRepository)
{
  public async Task<Result<CreateExpenseResult>> HandleAsync(CreateExpenseCommand command)
  {
    bool isMember = await groupMemberRepository.IsMemberAsync(command.RequestingUserId, command.GroupId);
    if (!isMember)
    {
      return Result<CreateExpenseResult>.Failure("Vous devez être membre du groupe pour creer une dépense.");
    }
    
    Domain.Entities.User? payer = await userRepository.GetUserByIdAsync(command.PayerId);
    if (payer is null)
    {
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
        var amount = amountsByUser[user.Id];
        userExpenseInfos.Add(new UserExpenseInfoResult(user.Id, user.Pseudo, part.Percentage, amount, user.Email));
      }
    }

    var result = new CreateExpenseResult(savedExpense, userExpenseInfos, payer.Pseudo, payer.Email);
    
    return Result<CreateExpenseResult>.Success(result);
  }
}