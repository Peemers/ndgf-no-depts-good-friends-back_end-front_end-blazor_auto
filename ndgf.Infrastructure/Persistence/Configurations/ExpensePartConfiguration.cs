using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ndgf.Domain.Entities;

namespace ndgf.Infrastructure.Persistence.Configurations;

public class ExpensePartConfiguration : IEntityTypeConfiguration<ExpensePart>
{
  public void Configure(EntityTypeBuilder<ExpensePart> builder)
  {
    builder.ToTable("ExpenseParts");

    builder.HasKey(ep => new { ep.ExpenseId, ep.UserId });
    
    builder.HasOne<User>()
      .WithMany()
      .HasForeignKey(e => e.UserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}