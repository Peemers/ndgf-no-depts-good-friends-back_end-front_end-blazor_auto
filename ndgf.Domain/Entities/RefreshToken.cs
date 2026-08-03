using ndgf.Domain.Common;
using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Entities;

public sealed partial class RefreshToken : BaseEntity
{
  public string Token { get; private set; } = null!;
  public Guid UserId { get; private set; }
  public DateTime ExpiresAt { get; private set; }
  public DateTime? RevokedAt { get; private set; }

  private RefreshToken()
  {
    
  }
  
  private RefreshToken(Guid id, string token, Guid userId, DateTime expiresAt, DateTime createdAt) : base(id, createdAt)
  {
    Token = token;
    UserId = userId;
    ExpiresAt = expiresAt;
  }

  public static RefreshToken Create(string token, Guid userId, DateTime expiresAt)
  {
    if (string.IsNullOrWhiteSpace(token))
    {
      throw new DomainException("Un token doit etre renseigné");
    }

    if (userId == Guid.Empty)
    {
      throw new DomainException("UserId requis");
    }

    if (expiresAt <= DateTime.UtcNow)
    {
      throw new DomainException("La date d'expiration ne doit pas être dans le passé");
    }
    
    return new RefreshToken(Guid.NewGuid(), token, userId, expiresAt, DateTime.UtcNow);
  }

  public void Revoke()
  {
    if (RevokedAt is not null)
    {
      throw new DomainException("Ce token a déjà été révoqué");
    }
    RevokedAt = DateTime.UtcNow;
  }
}