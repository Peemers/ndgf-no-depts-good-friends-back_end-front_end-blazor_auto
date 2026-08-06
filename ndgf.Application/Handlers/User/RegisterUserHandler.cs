using ndgf.Application.Commands.User;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Security;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.User;

public class RegisterUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
{
  public async Task<Result<Domain.Entities.User>> HandleAsync(RegisterUserCommand command)
  {
    bool emailAlreadyExists = await userRepository.EmailAlreadyExistsAsync(command.Email);
    if (emailAlreadyExists)
    {
      return Result<Domain.Entities.User>.Failure("Cet email est déja utilisé.");
    }
    
    bool pseudoAlreadyExists = await userRepository.PseudoAlreadyExistsAsync(command.Pseudo);
    if (pseudoAlreadyExists)
    {
      return Result<Domain.Entities.User>.Failure("Pseudo déjà utilisé");
    }

    string passwordHash = passwordHasher.HashPassword(command.Password);

    Domain.Entities.User user = Domain.Entities.User.Create(command.Email, passwordHash, command.Pseudo, command.LastName, command.FirstName);
    
    Domain.Entities.User savedUser = await userRepository.AddAsync(user);
    
    return Result<Domain.Entities.User>.Success(savedUser);
    
  }
}