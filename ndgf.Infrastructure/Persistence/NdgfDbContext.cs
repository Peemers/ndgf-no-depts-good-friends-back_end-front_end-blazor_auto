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
  public DbSet<Group> Groups => Set<Group>();
  public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
  public DbSet<Expense> Expenses => Set<Expense>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(NdgfDbContext).Assembly);
    
    base.OnModelCreating(modelBuilder);
  }
}