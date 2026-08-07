using ndgf.Web.Models.Group;

namespace ndgf.Web.Services.Group;

public class GroupApiClient(HttpClient httpClient)
{
  public async Task<HttpResponseMessage> CreateGroupAsync(CreateGroupeModel model)
  {
    return await httpClient.PostAsJsonAsync("/api/groups", model);
  }

  public async Task<HttpResponseMessage> AddGroupMember(AddGroupMemberModel model)
  {
    return await httpClient.PostAsJsonAsync("/api/groups/members", model);
  }
}