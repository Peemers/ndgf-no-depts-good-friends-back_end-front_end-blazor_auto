using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ndgf.Domain.Entities;

namespace ndgf.Infrastructure.Persistence.Configurations;

public class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
  public void Configure(EntityTypeBuilder<GroupMember> builder)
  {
    builder.ToTable("GroupMembers");
    
    builder.HasKey(ec => new { ec.GroupId, ec.UserId });
    
    builder.HasOne<Group>()
      .WithMany()
      .HasForeignKey(gm => gm.GroupId)
      .OnDelete(DeleteBehavior.Cascade);
    
    builder.HasOne<User>()
      .WithMany()
      .HasForeignKey(gm => gm.UserId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}