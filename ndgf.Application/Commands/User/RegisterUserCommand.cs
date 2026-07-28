namespace ndgf.Application.Commands.User;

public record RegisterUserCommand(string Email, string Password, string Pseudo, string FirstName, string LastName);