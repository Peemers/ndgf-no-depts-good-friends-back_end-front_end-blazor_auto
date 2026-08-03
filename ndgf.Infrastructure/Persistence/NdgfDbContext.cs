using Microsoft.EntityFrameworkCore;
using ndgf.Domain.Entities;

namespace ndgf.Infrastructure.Persistence;

public class NdgfDbContext : DbContext
{
  public NdgfDbContext(DbContextOptions<NdgfDbContext> options) : base(options)
  {
  }
  
  public DbSet<User> Users => Set<User>();
  public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(NdgfDbContext).Assembly);
    
    base.OnModelCreating(modelBuilder);
  }
}