using System.ComponentModel.DataAnnotations;

namespace ndgf.Api.Dtos.User.Request;

public record LoginUserRequestDto
{
  [Required]
  [RegularExpression(@"^[\w\-\.]+@([\w-]+\.)+[\w-]{2,}$", ErrorMessage = "Email invalide")]
  public required string Email { get; init; }
  
  [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
  public required string Password { get; init; }
}