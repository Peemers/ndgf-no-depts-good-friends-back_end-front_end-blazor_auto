using ndgf.Api.Dtos.User.Request;
using ndgf.Api.Dtos.User.Response;
using ndgf.Application.Commands.User;
using ndgf.Application.Models.User;

namespace ndgf.Api.Mappers;

public static class LoginUserMapper
{
  public static LoginUserCommand ToCommand(this LoginUserRequestDto dto)
  {
    return new LoginUserCommand(dto.Email, dto.Password);
  }

  public static LoginUserResponseDto ToResponseDto(this LoginResult result)
  {
    return new LoginUserResponseDto
    {
      Email = result.User.Email,
      Id = result.User.Id,
      AccessToken = result.AccessToken,
      RefreshToken = result.RefreshToken
    };
  }
}