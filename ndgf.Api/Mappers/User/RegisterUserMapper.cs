using ndgf.Api.Dtos.User.Request;
using ndgf.Api.Dtos.User.Response;
using ndgf.Application.Commands.User;

namespace ndgf.Api.Mappers.User;

public static class RegisterUserMapper
{
  public static RegisterUserCommand ToCommand(this RegisterUserRequestDto dto)
  {
    return new RegisterUserCommand(dto.Email, dto.Password, dto.Pseudo, dto.FirstName, dto.LastName);
  }

  public static RegisterUserResponseDto ToResponseDto(this Domain.Entities.User user)
  {
    return new RegisterUserResponseDto
    {
      Email = user.Email,
      Id = user.Id,
    };
  }
}