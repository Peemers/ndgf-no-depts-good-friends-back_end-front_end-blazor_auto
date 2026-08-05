using ndgf.Domain.Entities;

namespace ndgf.Application.Models.Group;

public record CreateGroupResult(Domain.Entities.Group Group, GroupMember GroupMember);