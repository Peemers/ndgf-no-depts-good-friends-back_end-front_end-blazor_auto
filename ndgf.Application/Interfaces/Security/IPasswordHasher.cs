namespace ndgf.Application.Interfaces.Security;

public interface IPasswordHasher
{
  //pas d'asynchronisme ici car "calcul" Bcrypt uniquement en mémoire.
  string HashPassword(string password);
  bool VerifyHashedPassword(string hashedPassword, string providedPassword);
}