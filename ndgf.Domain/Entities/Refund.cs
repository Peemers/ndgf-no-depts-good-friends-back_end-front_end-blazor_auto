using ndgf.Domain.Common;
using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Entities;

public sealed partial class Refund : BaseEntity
{
  public decimal Amount { get; private set; }
  public string? Description { get; private set; }
  public Guid PayerId { get; private set; }
  public Guid ReceiverId { get; private set; }
  public Guid GroupId { get; private set; }

  private Refund()
  {
  }

  private Refund(Guid id, DateTime createdAt, decimal amount, string? description, Guid payerId, Guid receiverId, Guid groupId)
    : base(id, createdAt)
  {
    Amount = amount;
    Description = description;
    PayerId = payerId;
    ReceiverId = receiverId;
    GroupId = groupId;
  }

  public static Refund Create(Guid payerId, Guid receiverId, decimal amount, string? description, Guid groupId)
  {
    if (payerId == Guid.Empty)
    {
      throw new DomainException("payerId requis");
    }

    if (payerId == receiverId)
    {
      throw new DomainException("Impossible de se rembourser soit même");
    }

    if (receiverId == Guid.Empty)
    {
      throw new DomainException("receiverId requis");
    }

    if (groupId == Guid.Empty)
    {
      throw new DomainException("groupId requis");
    }

    if (amount <= 0)
    {
      throw new DomainException("Le montant doit etre supérieur à 0");
    }
    
    var refundId = Guid.NewGuid();

    return new Refund(refundId, DateTime.UtcNow, amount, description, payerId, receiverId, groupId);
  }

  public void SoftDelete()
  {
    if (DeletedAt is not null)
    {
      throw new DomainException("Ce remboursement est déja supprimé");
    }
    MarkAsDeleted();
  }
}