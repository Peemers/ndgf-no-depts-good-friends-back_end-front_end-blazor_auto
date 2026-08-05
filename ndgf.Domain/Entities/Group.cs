using ndgf.Domain.Common;
using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Entities;

public sealed partial class Group : BaseEntity
{
  private const int NameMaxLength = 150;
  private const int NameMinLength = 3;
  private const int DescriptionMaxLength = 255;
  private const int DescriptionMinLength = 5;
  
  
  public string Name { get; private set; } = null!;
  public string? Description { get; private set; }
  public DateTime? ArchivedAt { get; private set; }

  private Group(){}

  private Group(Guid id, string name, string? description, DateTime createdAt) : base(id, createdAt)
  {
    Name = name;
    Description = description;
  }

  public static Group Create(string name, string? description)
  {
    if (string.IsNullOrEmpty(name))
    {
      throw new DomainException("Un nom de groupe doit être fourni");
    }

    if (name.Length < NameMinLength)
    {
      throw new DomainException($"Le nom du groupe doit comporter minimum {NameMinLength} caractères.");
    }

    if (name.Length > NameMaxLength)
    {
      throw new DomainException($"Le nom du groupe ne peut pas comporter plus de {NameMaxLength} caractères.");
    }

    if (description is not null && description.Length > DescriptionMaxLength)
    {
      throw new DomainException($"La description ne peut comporter plus de {DescriptionMaxLength} caractères");
    }
    
    if (description is not null && description.Length < DescriptionMinLength)
    {
      throw new DomainException($"La description doit comporter minimum {DescriptionMinLength} caractères");
    }

    return new Group(Guid.NewGuid(), name, description, DateTime.UtcNow);
  }

  public void Archive()
  {
    if (ArchivedAt is not null)
    {
      throw new DomainException("Ce groupe est déjà archivé");
    }
    ArchivedAt = DateTime.UtcNow;
  }
}
  