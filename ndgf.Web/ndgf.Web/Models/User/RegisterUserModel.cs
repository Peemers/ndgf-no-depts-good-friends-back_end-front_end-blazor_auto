using System.ComponentModel.DataAnnotations;

namespace ndgf.Web.Models.User;

public record RegisterUserModel
{
  [Required(ErrorMessage = "Le mail est obligatoire")]
  [RegularExpression(@"^[\w\-\.]+@([\w-]+\.)+[\w-]{2,}$", ErrorMessage = "Email invalide")]
  public required string Email { get; set; }

  [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
  [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères.")]
  [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$",
    ErrorMessage = "Le mot de passe doit contenir au moins 1 majuscule et 1 chiffre.")]
  public required string Password { get; set; }
  
  [Required(ErrorMessage = "Le mot de passe de confirmation est obligatoire.")]
  [Compare(nameof(Password), ErrorMessage = " Les mots de passe ne correspondent pas.")]
  public required string ConfirmPassword { get; set; }

  [Required(ErrorMessage = "Un pseudo est obligatoire.")]
  [MinLength(3, ErrorMessage = "Le pseudo doit comporter minimum 3 caractères")]
  [MaxLength(30, ErrorMessage = "Le pseudo doit comporter maximum 30 caractères")]
  public required string Pseudo { get; set; }

  [Required(ErrorMessage = "Le prénom est obligatoire.")]
  [MinLength(2, ErrorMessage = "Le prénom doit comporter minimum 2 caractères")]
  [MaxLength(30, ErrorMessage = "Le prénom doit comporter maximum 30 caractères")]
  public required string FirstName { get; set; }

  [Required(ErrorMessage = "Le nom est obligatoire.")]
  [MinLength(2, ErrorMessage = "Le nom doit comporter minimum 2 caractères")]
  [MaxLength(30, ErrorMessage = "Le nom doit comporter maximum 30 caractères")]
  public required string LastName { get; set; }
};