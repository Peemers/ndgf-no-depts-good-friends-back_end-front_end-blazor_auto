using System.Text.RegularExpressions;
using ndgf.Domain.Common;
using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Entities;

//Partial pour autoriser le GenerateRegex (source generator) d'écrire du code optimisé pour moi dans les fichiers de build et non exécuté au runtime (gain de performance)

public sealed partial class User : BaseEntity
{
  private const int PseudoMaxLength = 30;
  private const int FirstNameLastNameMaxLength = 30;
  private const int PseudoMinLength = 3;
  private const int FirstNameLastNameMinLength = 2;

  public string Email { get; private set; } = null!;
  public string PasswordHash { get; private set; } = null!;
  public string Pseudo { get; private set; } = null!;
  public string FirstName { get; private set; } = null!;
  public string LastName { get; private set; } = null!;

  private User()
  {
  }

  private User(Guid id, DateTime createdAt, string email, string passwordHash, string pseudo, string lastName, string firstName)  : base(id, createdAt)
  {
    Email = email;
    PasswordHash = passwordHash;
    Pseudo = pseudo;
    FirstName = firstName;
    LastName = lastName;
  }

  public static User Create(string email, string passwordHash, string pseudo, string lastName, string firstName)
  {
    if (string.IsNullOrWhiteSpace(email))
    {
      throw new DomainException("L'email est obligatoire.");
    }

    if (!EmailRegex().IsMatch(email))
    {
      throw new DomainException("Le format de l'émail est invalide");
    }

    if (string.IsNullOrWhiteSpace(passwordHash))
    {
      throw new DomainException("Le mot de passe est obligatoire.");
    }

    if (string.IsNullOrWhiteSpace(pseudo))
    {
      throw new DomainException("Le pseudo est obligatoire.");
    }

    if (pseudo.Length > PseudoMaxLength)
    {
      throw new DomainException($"Le pseudo ne peut pas dépasser {PseudoMaxLength} caractères.");
    }

    if (pseudo.Length < PseudoMinLength)
    {
      throw new DomainException($"Le pseudo ne peut comporter moins de {PseudoMinLength} caractères");
    }

    if (string.IsNullOrWhiteSpace(lastName))
    {
      throw new DomainException("Le nom est obligatoire.");
    }

    if (lastName.Length > FirstNameLastNameMaxLength)
    {
      throw new DomainException($"Le nom ne peut pas dépasser {FirstNameLastNameMaxLength} caractères.");
    }

    if (lastName.Length < FirstNameLastNameMinLength)
    {
      throw new DomainException($"Le nom ne peut pas comporter moins de {FirstNameLastNameMinLength}");
    }

    if (string.IsNullOrWhiteSpace(firstName))
    {
      throw new DomainException("Le prénom est obligatoire.");
    }

    if (firstName.Length > FirstNameLastNameMaxLength)
    {
      throw new DomainException($"Le prénom ne peut pas comporter plus de {FirstNameLastNameMaxLength} caractères.");
    }

    if (firstName.Length < FirstNameLastNameMinLength)
    {
      throw new DomainException($"Le prénom ne peut pas comporter moins de {FirstNameLastNameMinLength} caractères");
    }

    return new User(Guid.NewGuid(), DateTime.UtcNow, email, passwordHash, pseudo, lastName, firstName);
  }
  [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
  private static partial Regex EmailRegex();
}