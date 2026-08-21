using ndgf.Api.Dtos.Expense.Response;
using ndgf.Application.Models.Expense;

namespace ndgf.Api.Mappers.Expense;

public static class GetGroupBalanceMapper
{
  public static GetGroupBalanceResponseDto ToResponseDto(this GetGroupBalanceResult result)
  {
    var balances = result.Balances.Select(b => new UserBalanceResponseDto
    {
      UserId = b.UserId,
      Pseudo = b.Pseudo,
      Balance = b.Balance
    }).ToList();

    return new GetGroupBalanceResponseDto { Balances = balances };
  }
}