using ndgf.Api.Dtos.Expense.Request;
using ndgf.Api.Dtos.Expense.Response;
using ndgf.Application.Commands.Expense;
using ndgf.Application.Models.Expense;
using ndgf.Domain.Common;

namespace ndgf.Api.Mappers.Expense;

public static class CreateExpenseMapper
{
  public static CreateExpenseCommand ToCommand(this CreateExpenseRequestDto dto, Guid requestingUserId, Guid groupId)
  {
    var expensePartInputs = dto.ExpenseParts
      .Select(ep => new ExpensePartInput(ep.UserId, ep.Percentage))
      .ToList();

    return new CreateExpenseCommand(requestingUserId, dto.UserId, expensePartInputs, dto.Amount, dto.Description, groupId);
  }

  public static CreateExpenseResponseDto ToResponseDto(this CreateExpenseResult result)
  {
    return new CreateExpenseResponseDto
    {
      Id = result.Expense.Id,
      Amount = result.Expense.Amount,
      Description = result.Expense.Description,
      PayerPseudo = result.PayerPseudo,
      PayerEmail = result.PayerEmail,
      UserExpenseInfos = result.UserExpenseInfoResults.Select(info => new UserExpenseInfoResponseDto
      {
        UserId = info.UserId,
        Pseudo = info.Pseudo,
        Percentage = info.Percentage,
        Amount = info.Amount,
        Email = info.Email
      }).ToList()
    };
  }
}