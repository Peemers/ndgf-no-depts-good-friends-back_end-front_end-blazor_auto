namespace ndgf.Application.Commands.Refund;

public record SoftDeleteRefundCommand(Guid UserId, Guid GroupId, Guid RefundId);