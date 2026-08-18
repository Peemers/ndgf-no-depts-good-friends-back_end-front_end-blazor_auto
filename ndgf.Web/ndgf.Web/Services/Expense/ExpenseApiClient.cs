using ndgf.Web.Models.Expense;

namespace ndgf.Web.Services.Expense;

public class ExpenseApiClient(HttpClient httpClient)
{
  public async Task<HttpResponseMessage> CreateExpenseAsync(CreateExpenseRequestModel model, Guid groupId)
  {
    return await httpClient.PostAsJsonAsync($"/api/groups/{groupId}/expenses", model);
  }
}