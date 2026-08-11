using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ndgf.Domain.Entities;

namespace ndgf.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.ToTable("Users");
    
    builder.HasKey(u => u.Id);
    
    builder.Property(u => u.Email)
      .HasMaxLength(256)
      .IsRequired();
    
    builder.HasIndex(u => u.Email)
      .IsUnique();
    
    builder.Property(u => u.PasswordHash)
      .IsRequired();

    builder.Property(u => u.Pseudo)
      .IsRequired()
      .HasMaxLength(30);
    
    builder.HasIndex(u => u.Pseudo)
      .IsUnique();
    
    builder.Property(u => u.FirstName)
      .IsRequired()
      .HasMaxLength(30);
    
    builder.Property(u => u.LastName)
      .IsRequired()
      .HasMaxLength(30);
  }
}