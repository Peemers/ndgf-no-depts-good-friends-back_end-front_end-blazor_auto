using ndgf.Web.Models.Refund;

namespace ndgf.Web.Services.Refund;

public class RefundApiClient(HttpClient httpClient)
{
  public async Task<HttpResponseMessage> CreateRefundAsync(CreateRefundRequestModel model, Guid groupId)
  {
    return await httpClient.PostAsJsonAsync($"/api/groups/{groupId}/refunds", model);
  }

  public async Task<HttpResponseMessage> DeleteRefundAsync(Guid groupId, Guid refundId)
  {
    return await httpClient.DeleteAsync($"/api/groups/{groupId}/refunds/{refundId}");
  }
}