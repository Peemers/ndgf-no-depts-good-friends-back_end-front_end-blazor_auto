using Microsoft.Extensions.Logging;
using ndgf.Application.Handlers.GroupHistory;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.GroupHistory;
using ndgf.Application.Queries.GroupHistory;
using ndgf.Domain.Common;
using NSubstitute;

namespace ndgf.Application.Tests.GroupHistory;

public class GetGroupHistoryHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithValidData_ShouldReturnMergedAndSortedTransactions()
    {
        IUserRepository userRepository = Substitute.For<IUserRepository>();
        IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
        IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
        IRefundRepository refundRepository = Substitute.For<IRefundRepository>();
        ILogger<GetGroupHistoryHandler> logger = Substitute.For<ILogger<GetGroupHistoryHandler>>();

        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var payerId = Guid.NewGuid();

        var expensePartInput = new List<ExpensePartInput> { new(userId, 100) };
        var expense = Domain.Entities.Expense.Create(payerId, expensePartInput, 100m, "Resto", groupId);
        var refund = Domain.Entities.Refund.Create(userId, payerId, 50m, "Remboursement", groupId);

        var expectedUser = Domain.Entities.User.Create("test@test.be", "Test1234!", "Toto", "Jack", "Leonardo");

        groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
        expenseRepository.GetAllGroupExpensesAsync(Arg.Any<Guid>()).Returns(new List<Domain.Entities.Expense> { expense });
        refundRepository.GetAllGroupRefundAsync(Arg.Any<Guid>()).Returns(new List<Domain.Entities.Refund> { refund });
        userRepository.GetUserByIdAsync(Arg.Any<Guid>()).Returns(expectedUser);

        var handler = new GetGroupHistoryHandler(userRepository, groupMemberRepository, expenseRepository, refundRepository, logger);
        var query = new GetGroupHistoryQuery(groupId, userId, 1, 10, true);

        var result = await handler.HandleAsync(query);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.TransactionSummary.TotalCount);
        Assert.Equal(2, result.Value.TransactionSummary.Items.Count());
        var refundSummary = result.Value.TransactionSummary.Items.First(t => t.Type == TransactionType.Refund);
        Assert.Equal(expectedUser.Pseudo, refundSummary.ReceiverPseudo);
    }

    [Fact]
    public async Task HandleAsync_WithUserNotMember_ShouldReturnFailure()
    {
        IUserRepository userRepository = Substitute.For<IUserRepository>();
        IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
        IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
        IRefundRepository refundRepository = Substitute.For<IRefundRepository>();
        ILogger<GetGroupHistoryHandler> logger = Substitute.For<ILogger<GetGroupHistoryHandler>>();

        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false);

        var handler = new GetGroupHistoryHandler(userRepository, groupMemberRepository, expenseRepository, refundRepository, logger);
        var query = new GetGroupHistoryQuery(groupId, userId, 1, 10, true);

        var result = await handler.HandleAsync(query);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
    }
}