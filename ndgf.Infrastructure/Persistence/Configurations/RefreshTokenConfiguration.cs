using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ndgf.Domain.Entities;

namespace ndgf.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
  public void Configure(EntityTypeBuilder<RefreshToken> builder)
  {
    builder.ToTable("RefreshTokens");
    
    builder.HasKey(r => r.Id);
    
    builder.Property(r => r.Token)
      .IsRequired()
      .HasMaxLength(88);

    builder.Property(r => r.ExpiresAt)
      .IsRequired();

    builder.HasIndex(r => r.Token)
      .IsUnique();

    builder.HasOne<User>()
      .WithMany()
      .HasForeignKey(r => r.UserId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}