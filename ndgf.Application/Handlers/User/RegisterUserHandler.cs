using ndgf.Application.Commands.User;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Security;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.User;

public class RegisterUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
{
  public async Task<Result<Domain.Entities.User>> HandleAsync(RegisterUserCommand command)
  {
    var emailAlreadyExists = await userRepository.EmailAlreadyExistsAsync(command.Email);
    if (emailAlreadyExists)
    {
      return Result<Domain.Entities.User>.Failure("Cet emil est déja utilisé.");
    }

    var passwordHash = passwordHasher.HashPassword(command.Password);

    var user = Domain.Entities.User.Create(command.Email, passwordHash, command.Pseudo, command.LastName, command.FirstName);
    
    var savedUser = await userRepository.AddAsync(user);
    
    return Result<Domain.Entities.User>.Success(savedUser);
  }
}