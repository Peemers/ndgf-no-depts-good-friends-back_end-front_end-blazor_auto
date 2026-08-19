using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ndgf.Domain.Entities;

namespace ndgf.Infrastructure.Persistence.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
  public void Configure(EntityTypeBuilder<Expense> builder)
  {
    builder.ToTable("Expenses");
    
    builder.HasKey(e => e.Id);

    builder.Property(e => e.Amount)
      .IsRequired()
      .HasPrecision(18, 2);

    builder.Property(e => e.Description)
      .IsRequired()
      .HasMaxLength(256);
    
    builder.HasMany(e => e.ExpenseParts)
      .WithOne()
      .HasForeignKey(ep => ep.ExpenseId)
      .OnDelete(DeleteBehavior.Cascade);
    
    builder.HasOne<User>()
      .WithMany()
      .HasForeignKey(e => e.UserId)
      .OnDelete(DeleteBehavior.Restrict);
    
    builder.HasOne<Group>()
      .WithMany()
      .HasForeignKey(e => e.GroupId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}