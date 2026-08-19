using ndgf.Web.Models.Expense;

namespace ndgf.Web.Services.Expense;

public class ExpenseApiClient(HttpClient httpClient)
{
  public async Task<HttpResponseMessage> CreateExpenseAsync(CreateExpenseRequestModel model, Guid groupId)
  {
    return await httpClient.PostAsJsonAsync($"/api/groups/{groupId}/expenses", model);
  }

  public async Task<HttpResponseMessage> GetGroupExpensesAsync(Guid groupId, int pageNumber, int pageSize, bool sortDescending)
  {
    return await httpClient.GetAsync($"/api/groups/{groupId}/expenses?page={pageNumber}&pageSize={pageSize}&sortDescending={sortDescending}");
  }
}