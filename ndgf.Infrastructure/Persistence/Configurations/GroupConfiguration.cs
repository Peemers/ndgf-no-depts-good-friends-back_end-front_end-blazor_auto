using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ndgf.Domain.Entities;

namespace ndgf.Infrastructure.Persistence.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
  public void Configure(EntityTypeBuilder<Group> builder)
  {
    builder.ToTable("Groups");
    
    builder.HasKey(g => g.Id);
    
    builder.Property(g => g.Name)
      .IsRequired()
      .HasMaxLength(150);
    
    builder.Property(g => g.Description)
      .HasMaxLength(255);

    builder.Property(g => g.ArchivedAt);
  }
}