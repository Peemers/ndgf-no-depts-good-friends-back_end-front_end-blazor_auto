using System.ComponentModel.DataAnnotations;

namespace ndgf.Api.Dtos.Group.Request;


public record AddGroupMemberRequestDto
{
  [Required (ErrorMessage = "Entrez un email ou un pseudo")]
  public required string SearchValue { get; init; }
}