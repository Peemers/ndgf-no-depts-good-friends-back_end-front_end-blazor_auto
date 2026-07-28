namespace ndgf.Domain.Common;

public abstract class BaseEntity
{
  public Guid Id { get; protected set; } // protected pour autoriser les enfants à écrire dans ces champs. Pas oublier.
  public DateTime CreatedAt { get; protected set; }
  public DateTime? UpdatedAt { get; protected set; }

  protected BaseEntity()
  {
    
  }
  
  protected BaseEntity(Guid id, DateTime createdAt)
  {
    Id = id;
    CreatedAt = createdAt;
  }
  
  protected void MarkAsUpdated()
  {
    UpdatedAt = DateTime.UtcNow;
  }
}