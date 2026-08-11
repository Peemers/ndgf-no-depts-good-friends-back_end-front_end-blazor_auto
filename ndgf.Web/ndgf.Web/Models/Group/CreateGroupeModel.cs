using System.ComponentModel.DataAnnotations;

namespace ndgf.Web.Models.Group;

public record CreateGroupeModel
{
  [Required]
  [MinLength(3, ErrorMessage = "Le nom doit comporter minimum 3 caractères")]
  [MaxLength(150, ErrorMessage = "Le nom ne peut comporter plus de 150 caractères")]
  public required string Name { get; set; }
  
  [MinLength(5, ErrorMessage = "La déscription doit comporter minimum 5 caractères")]
  [MaxLength(255, ErrorMessage = "La déscription ne peut comporter plus de 255 caractères")]
  public string? Description { get; set; }
}