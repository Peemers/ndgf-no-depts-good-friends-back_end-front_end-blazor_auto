using System.ComponentModel.DataAnnotations;

namespace ndgf.Web.Models.Group;

public record AddGroupMemberModel
{
  [Required(ErrorMessage = "Entrez un mail ou un pseudo")]
  public required string SearchValue { get; set; }
}