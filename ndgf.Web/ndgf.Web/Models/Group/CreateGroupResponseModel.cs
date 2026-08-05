namespace ndgf.Web.Models.Group;

public class CreateGroupResponseModel
{
  public Guid Id { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
}