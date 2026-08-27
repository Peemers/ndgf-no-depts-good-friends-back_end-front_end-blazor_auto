using ndgf.Application.Commands.Refund;
using ndgf.Application.Handlers.Refund;
using ndgf.Application.Interfaces.Repositories;
using NSubstitute;
namespace ndgf.Application.Tests.Refund;

public class SoftDeleteRefundHandlerTests
{
  [Fact]
  public async Task HandleAsync_WithValidData_ShouldReturnSucceed()
  {
    IRefundRepository refundRepository = Substitute.For<IRefundRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    
    var userId = Guid.NewGuid();
    var refundId = Guid.NewGuid();
    var receiverId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var amount = 250m;
    var description = "Test refund";

    var expectedRefund = Domain.Entities.Refund.Create(payerId, receiverId, amount, description, groupId);
    
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    refundRepository.GetRefundByIdAsync(Arg.Any<Guid>()).Returns(expectedRefund);
    
    var handler = new SoftDeleteRefundHandler(refundRepository, groupMemberRepository);
    var command = new SoftDeleteRefundCommand(userId, groupId, refundId);
    var result = await handler.HandleAsync(command);
    
    Assert.True(result.IsSuccess);
    Assert.NotNull(expectedRefund.DeletedAt);
    await  refundRepository.Received(1).UpdateAsync(expectedRefund);
  }

  [Fact]
  public async Task HandleAsync_WithMemberNotInGroup_ShouldReturnFailureResult()
  {
    IRefundRepository refundRepository = Substitute.For<IRefundRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    
    var userId = Guid.NewGuid();
    var refundId = Guid.NewGuid();
    var groupId =  Guid.NewGuid();
    
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false);
    
    var handler = new SoftDeleteRefundHandler(refundRepository, groupMemberRepository);
    var command = new SoftDeleteRefundCommand(userId, groupId, refundId);
    var result = await handler.HandleAsync(command);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    await refundRepository.DidNotReceiveWithAnyArgs().UpdateAsync(Arg.Any<Domain.Entities.Refund>());
  }

  [Fact]
  public async Task HandleAsync_WithRefundNotFound_ShouldReturnFailureResult()
  {
    IRefundRepository refundRepository = Substitute.For<IRefundRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    
    var userId = Guid.NewGuid();
    var refundId = Guid.NewGuid();
    var groupId =  Guid.NewGuid();
    
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    refundRepository.GetRefundByIdAsync(Arg.Any<Guid>()).Returns((Domain.Entities.Refund?)null);
    
    var handler = new SoftDeleteRefundHandler(refundRepository, groupMemberRepository);
    var command = new SoftDeleteRefundCommand(userId, groupId, refundId);
    var result = await handler.HandleAsync(command);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    await refundRepository.DidNotReceiveWithAnyArgs().UpdateAsync(Arg.Any<Domain.Entities.Refund>());
  }
}