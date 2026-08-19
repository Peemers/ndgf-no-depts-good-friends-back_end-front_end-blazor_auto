using ndgf.Domain.Common;
using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Entities;

public sealed partial class Expense : BaseEntity
{
  public decimal Amount { get; private set; }
  public string Description { get; private set; } = null!;
  public Guid UserId { get; private set; }
  public Guid GroupId { get; private set; }

  private readonly List<ExpensePart> _expenseParts = [];

  public IReadOnlyList<ExpensePart> ExpenseParts => _expenseParts;
  
  private Expense(){}

  private Expense(Guid id, DateTime createdAt, decimal amount, string description, Guid userId, Guid groupId, List<ExpensePart> expenseParts)
    : base(id, createdAt)
  {
    Amount = amount;
    Description = description;
    UserId = userId;
    GroupId = groupId;
    _expenseParts = expenseParts;
  }

  public static Expense Create(Guid userId, List<ExpensePartInput> expensesPartsInput, decimal amount, string description, Guid groupId)
  {
    if (userId == Guid.Empty)
    {
      throw new DomainException("UserId requis");
    }

    if (groupId == Guid.Empty)
    {
      throw new DomainException("GroupId est requis");
    }

    if (amount <= 0)
    {
      throw new DomainException("Le montant doit etre supérieur à 0");
    }

    if (string.IsNullOrWhiteSpace(description))
    {
      throw new DomainException("Une déscription est requise");
    }
    
    var totalPercentage = expensesPartsInput.Sum(input => input.Percentage);
    
    if (totalPercentage != 100)
    {
      throw new DomainException("La somme des pourcentages doit être égale à 100");
    }
    
    var expenseId = Guid.NewGuid();

    var expenseParts = expensesPartsInput
      .Select(input => ExpensePart.Create(input.UserId, expenseId, input.Percentage)).ToList();
    
    return new Expense(expenseId, DateTime.UtcNow, amount, description, userId, groupId, expenseParts);
  }

  public Dictionary<Guid, decimal> CalculateAmountsByUser()
  {
    return _expenseParts.ToDictionary(
      part => part.UserId,
      part => Amount * part.Percentage / 100m);
  }
}