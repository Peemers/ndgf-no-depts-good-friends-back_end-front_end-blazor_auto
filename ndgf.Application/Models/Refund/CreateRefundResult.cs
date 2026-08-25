namespace ndgf.Application.Models.Refund;

public record CreateRefundResult(Domain.Entities.Refund Refund, string PayerPseudo, string ReceiverPseudo);