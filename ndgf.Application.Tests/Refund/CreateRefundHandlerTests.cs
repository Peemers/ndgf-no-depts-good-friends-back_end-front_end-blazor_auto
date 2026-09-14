using Microsoft.Extensions.Logging;
using ndgf.Application.Commands.Refund;
using ndgf.Application.Handlers.Refund;
using ndgf.Application.Interfaces.Repositories;
using NSubstitute;
namespace ndgf.Application.Tests.Refund;

public class CreateRefundHandlerTests
{
  [Fact]
  public async Task HandleAsync_WithValidData_ShouldReturnSuccessResult()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IRefundRepository refundRepository = Substitute.For<IRefundRepository>();
    IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    ILogger<CreateRefundHandler> logger = Substitute.For<ILogger<CreateRefundHandler>>();
    
    var payerId = Guid.NewGuid();
    var receiverId = Guid.NewGuid();
    var requestingUserId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var amount = 100m;
    var description = "test";

    var expectedPayer = Domain.Entities.User.Create("test@test.be", "Test1234!", "Toto", "Tony", "Montana");
    var expectedReceiver = Domain.Entities.User.Create("test2@test.be", "Test1234!", "Toto", "Tonio", "Montana");
    
    groupRepository.IsArchivedAsync(Arg.Any<Guid>()).Returns(false);
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    userRepository.GetUserByIdAsync(payerId).Returns(expectedPayer);
    userRepository.GetUserByIdAsync(receiverId).Returns(expectedReceiver);
    var expectedRefund = Domain.Entities.Refund.Create(payerId, receiverId, amount, description, groupId);
    refundRepository.AddAsync(Arg.Any<Domain.Entities.Refund>()).Returns(expectedRefund);
    
    var handler = new CreateRefundHandler(userRepository, groupMemberRepository, refundRepository, groupRepository, logger);
    var command = new CreateRefundCommand(requestingUserId, payerId, receiverId, amount, description, groupId);
    
    var result = await handler.HandleAsync(command);
    
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    
    Assert.Equal(expectedPayer.Pseudo, result.Value.PayerPseudo);
    Assert.Equal(expectedReceiver.Pseudo, result.Value.ReceiverPseudo);
    Assert.Equal(amount, result.Value.Refund.Amount);
    Assert.Equal(description, result.Value.Refund.Description);
    Assert.Equal(groupId, result.Value.Refund.GroupId);
    Assert.Equal(payerId, result.Value.Refund.PayerId);
    Assert.Equal(receiverId, result.Value.Refund.ReceiverId);
  }

  [Fact]
  public async Task HandleAsync_WithUserNotMember_ShouldReturnFailureResult()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IRefundRepository refundRepository = Substitute.For<IRefundRepository>();
    IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    ILogger<CreateRefundHandler> logger = Substitute.For<ILogger<CreateRefundHandler>>();
    
    var requestingUserId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var receiverId = Guid.NewGuid();
    var amount = 100m;
    var description = "test";
    
    groupRepository.IsArchivedAsync(Arg.Any<Guid>()).Returns(false);
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false);
    
    var handler = new CreateRefundHandler(userRepository, groupMemberRepository, refundRepository, groupRepository, logger);
    var command = new CreateRefundCommand(requestingUserId, payerId, receiverId, amount, description, groupId);
    
    var result = await handler.HandleAsync(command);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }

  [Fact]
  public async Task HandleAsync_WithPayerNotFound_ShouldReturnFailureResult()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IRefundRepository refundRepository = Substitute.For<IRefundRepository>();
    IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    ILogger<CreateRefundHandler> logger = Substitute.For<ILogger<CreateRefundHandler>>();
    
    var requestingUserId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var receiverId = Guid.NewGuid();
    var amount = 100m;
    var description = "test";
    
    groupRepository.IsArchivedAsync(Arg.Any<Guid>()).Returns(false);
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    userRepository.GetUserByIdAsync(payerId).Returns((Domain.Entities.User?)null);
    
    var handler = new CreateRefundHandler(userRepository, groupMemberRepository, refundRepository, groupRepository, logger);
    var command = new CreateRefundCommand(requestingUserId, payerId, receiverId, amount, description, groupId);
    
    var result = await handler.HandleAsync(command);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }

  [Fact]
  public async Task HandleAsync_WithReceiverNotFound_ShouldReturnFailureResult()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IRefundRepository refundRepository = Substitute.For<IRefundRepository>();
    IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    ILogger<CreateRefundHandler> logger = Substitute.For<ILogger<CreateRefundHandler>>();
    
    var requestingUserId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var receiverId = Guid.NewGuid();
    var amount = 100m;
    var description = "test";

    var expectedPayer = Domain.Entities.User.Create("test@test.be", "Test1234!", "Toto", "Tony", "Montana");
    
    groupRepository.IsArchivedAsync(Arg.Any<Guid>()).Returns(false);
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    userRepository.GetUserByIdAsync(payerId).Returns(expectedPayer);
    userRepository.GetUserByIdAsync(receiverId).Returns((Domain.Entities.User?)null);
    
    var handler = new CreateRefundHandler(userRepository, groupMemberRepository, refundRepository, groupRepository, logger);
    var command = new CreateRefundCommand(requestingUserId, payerId, receiverId, amount, description, groupId);
    
    var result = await handler.HandleAsync(command);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }

  [Fact]
  public async Task HandleAsync_WithGroupAlreadyArchived_ShouldReturnFailureResult()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IRefundRepository refundRepository = Substitute.For<IRefundRepository>();
    IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    ILogger<CreateRefundHandler> logger = Substitute.For<ILogger<CreateRefundHandler>>();
    
    var requestingUserId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var receiverId = Guid.NewGuid();
    var amount = 100m;
    var description = "test";
    
    groupRepository.IsArchivedAsync(Arg.Any<Guid>()).Returns(true);
    
    var handler = new CreateRefundHandler(userRepository, groupMemberRepository, refundRepository, groupRepository, logger);
    var command = new CreateRefundCommand(requestingUserId, payerId, receiverId, amount, description, groupId);
    
    var result = await handler.HandleAsync(command);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }
}