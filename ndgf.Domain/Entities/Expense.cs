using ndgf.Domain.Common;
using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Entities;

public sealed partial class Expense : BaseEntity
{
  public decimal Amount { get; private set; }
  public string Description { get; private set; } = null!;
  public Guid UserId { get; private set; }
  public Guid GroupId { get; private set; }
  public Decimal? Latitude {get; private set;}
  public decimal? Longitude {get; private set;}
  public string? Location {get; private set;}

  private readonly List<ExpensePart> _expenseParts = [];

  public IReadOnlyList<ExpensePart> ExpenseParts => _expenseParts;
  
  private Expense(){}

  private Expense(Guid id, DateTime createdAt, decimal amount, string description, Guid userId, Guid groupId, List<ExpensePart> expenseParts, decimal? latitude,  decimal? longitude, string? location)
    : base(id, createdAt)
  {
    Amount = amount;
    Description = description;
    UserId = userId;
    GroupId = groupId;
    _expenseParts = expenseParts;
    Latitude = latitude;
    Longitude = longitude;
    Location = location;
  }

  public static Expense Create(Guid userId, List<ExpensePartInput> expensesPartsInput, decimal amount, string description, Guid groupId, decimal? latitude = null, decimal? longitude = null, string? location = null)
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

    if (latitude.HasValue && (latitude < -90 || latitude > 90))
    {
      throw new DomainException("La latitude doit etre comprise entre -90 et 90 degrés");
    }

    if (longitude.HasValue && (longitude < -180 || longitude > 180))
    {
      throw new DomainException("La longitude doit etre comprise entre -180 et 180 degrés");
    }
    
    var expenseId = Guid.NewGuid();

    var expenseParts = expensesPartsInput
      .Select(input => ExpensePart.Create(input.UserId, expenseId, input.Percentage)).ToList();
    
    return new Expense(expenseId, DateTime.UtcNow, amount, description, userId, groupId, expenseParts, latitude, longitude, location);
  }

  public Dictionary<Guid, decimal> CalculateAmountsByUser()
  {
    return _expenseParts.ToDictionary(
      part => part.UserId,
      part => Amount * part.Percentage / 100m);
  }

  public void SoftDelete()
  {
    if (DeletedAt is not null)
    {
      throw new DomainException("Cette dépense est deja supprimée");
    }
    MarkAsDeleted();
  }
}