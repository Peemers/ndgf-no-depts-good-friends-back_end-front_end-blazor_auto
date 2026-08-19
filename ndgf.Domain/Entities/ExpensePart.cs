using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Entities;

public class ExpensePart
{
  public Guid UserId { get; private set; }
  public Guid ExpenseId { get; private set; }
  public int Percentage { get; private set; }
  
  private ExpensePart(){}

  private ExpensePart(Guid userId, Guid expenseId, int percentage)
  {
    UserId = userId;
    ExpenseId = expenseId;
    Percentage = percentage;
  }

  public static ExpensePart Create(Guid userId, Guid expenseId, int percentage)
  {
    if (userId == Guid.Empty)
    {
      throw new DomainException("UserId requis");
    }

    if (expenseId == Guid.Empty)
    {
      throw new DomainException("ExpenseId requis");
    }

    if (percentage < 0)
    {
      throw new DomainException("Le pourcentage doit se trouver entre 0 et 100");
    }

    if (percentage > 100)
    {
      throw new DomainException("Le pourcentage doit se trouver entre 0 et 100");
    }

    return new ExpensePart(userId, expenseId, percentage);
  }
}