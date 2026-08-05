using System.ComponentModel.DataAnnotations;

namespace ndgf.Web.Models.User;

public class LoginUserModel
{
  [Required]
  [RegularExpression(@"^[\w\-\.]+@([\w-]+\.)+[\w-]{2,}$", ErrorMessage = "Email invalide")]
  public required string Email { get; set; }
  
  [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
  public required string Password { get; set; }
}