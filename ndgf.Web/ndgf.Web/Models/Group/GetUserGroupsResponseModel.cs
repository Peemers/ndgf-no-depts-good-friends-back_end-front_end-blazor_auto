namespace ndgf.Web.Models.Group;

public record GetUserGroupsResponseModel
{
  public required List<UserGroupSummaryModel> Groups { get; init; }
}