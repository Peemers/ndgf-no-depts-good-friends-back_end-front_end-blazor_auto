using System.ComponentModel.DataAnnotations;

namespace ndgf.Web.Models.Group;

public class AddGroupMemberModel
{
  [Required]
  public required string SearchValue { get; set; }
}