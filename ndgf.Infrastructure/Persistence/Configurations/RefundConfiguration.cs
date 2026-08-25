using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ndgf.Domain.Entities;

namespace ndgf.Infrastructure.Persistence.Configurations;

public class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
  public void Configure(EntityTypeBuilder<Refund> builder)
  {
    builder.ToTable("Refunds");
    
    builder.HasKey(r => r.Id);

    builder.Property(r => r.Amount)
      .IsRequired()
      .HasPrecision(18, 2);

    builder.Property(r => r.Description)
      .HasMaxLength(256);
    
    builder.HasOne<User>()
      .WithMany()
      .HasForeignKey(r => r.PayerId)
      .OnDelete(DeleteBehavior.Restrict);
    
    builder.HasOne<User>()
      .WithMany()
      .HasForeignKey(r => r.ReceiverId)
      .OnDelete(DeleteBehavior.Restrict);
    
    builder.HasOne<Group>()
      .WithMany()
      .HasForeignKey(r => r.GroupId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}