namespace ndgf.Web.Services.Auth;

public class TokenStore
{
  private readonly SemaphoreSlim _refreshLock = new(1, 1);
  public string? AccessToken { get; set; }
  public string? RefreshToken { get; set; }

  public async Task<IDisposable> LockAsync()
  {
    await _refreshLock.WaitAsync();
    return new Releaser(_refreshLock);
  }

  private class Releaser(SemaphoreSlim semaphore) : IDisposable
  {
    public void Dispose() => semaphore.Release();
  }
}