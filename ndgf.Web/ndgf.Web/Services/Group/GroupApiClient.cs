using ndgf.Web.Models.Group;

namespace ndgf.Web.Services.Group;

public class GroupApiClient(HttpClient httpClient)
{
  public async Task<HttpResponseMessage> CreateGroupAsync(CreateGroupeModel model)
  {
    return await httpClient.PostAsJsonAsync("/api/groups", model);
  }

  public async Task<HttpResponseMessage> AddGroupMemberAsync(AddGroupMemberModel model, Guid groupId)
  {
    return await httpClient.PostAsJsonAsync($"/api/groups/{groupId}/members", model);
  }

  public async Task<HttpResponseMessage> GetGroupDetailsAsync(Guid groupId)
  {
    return await httpClient.GetAsync($"/api/groups/{groupId}");
  }

  public async Task<HttpResponseMessage> GetUserGroupsAsync()
  {
    return await httpClient.GetAsync($"/api/groups/mine");
  }
}