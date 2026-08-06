using System.ComponentModel.DataAnnotations;

namespace ndgf.Api.Dtos.Group.Request;


public record AddGroupMemberRequestDto
{
  [Required]
  public required string SearchValue { get; init; }
}