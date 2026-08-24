namespace ndgf.Application.Commands.Refund;

public record CreateRefundCommand(Guid RequestingUserId, Guid PayerId, Guid ReceiverId, decimal Amount, string? Description, Guid GroupId);