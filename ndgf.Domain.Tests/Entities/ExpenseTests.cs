using ndgf.Domain.Common;
using ndgf.Domain.Entities;
using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Tests.Entities;

public class ExpenseTests
{
  [Fact]
  public void Create_WithValidDate_ShouldCreateExpense()
  {
    var userId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var amount = 100m;
    var description = "description";

    var user1Id = Guid.NewGuid();
    var user2Id = Guid.NewGuid();

    var expensePartInput = new List<ExpensePartInput>()
    {
      new(user1Id, 60),
      new(user2Id, 40)
    };

    var expense = Expense.Create(userId, expensePartInput, amount, description, groupId);

    Assert.Equal(userId, expense.UserId);
    Assert.Equal(amount, expense.Amount);
    Assert.Equal(description, expense.Description);
    Assert.Equal(groupId, expense.GroupId);
    Assert.Equal(2, expense.ExpenseParts.Count);
    Assert.Equal(user1Id, expense.ExpenseParts[0].UserId);
    Assert.Equal(60, expense.ExpenseParts[0].Percentage);
    Assert.Equal(user2Id, expense.ExpenseParts[1].UserId);
    Assert.Equal(40, expense.ExpenseParts[1].Percentage);
  }

  [Fact]
  public void Create_WithInvalidUserId_ShouldThrowException()
  {
    var groupId = Guid.NewGuid();
    var expensePartInput = new List<ExpensePartInput> { new(Guid.NewGuid(), 100) };

    Assert.Throws<DomainException>(() => Expense.Create(Guid.Empty, expensePartInput, 100m, "description", groupId));
  }

  [Fact]
  public void Create_WithInvalidGroupId_ShouldThrowException()
  {
    var userId = Guid.NewGuid();
    var expensePartInput = new List<ExpensePartInput> { new(Guid.NewGuid(), 100) };

    Assert.Throws<DomainException>(() => Expense.Create(userId, expensePartInput, 100m, "description", Guid.Empty));
  }

  [Fact]
  public void Create_WithZeroAmount_ShouldThrowException()
  {
    var userId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var expensePartInput = new List<ExpensePartInput> { new(Guid.NewGuid(), 100) };

    Assert.Throws<DomainException>(() => Expense.Create(userId, expensePartInput, 0m, "description", groupId));
  }

  [Fact]
  public void Create_WithNegativeAmount_ShouldThrowException()
  {
    var userId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var expensePartInput = new List<ExpensePartInput> { new(Guid.NewGuid(), 100) };

    Assert.Throws<DomainException>(() => Expense.Create(userId, expensePartInput, -50m, "description", groupId));
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Create_WithInvalidDescription_ShouldThrowException(string invalidDescription)
  {
    var userId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var expensePartInput = new List<ExpensePartInput> { new(Guid.NewGuid(), 100) };

    Assert.Throws<DomainException>(() => Expense.Create(userId, expensePartInput, 100m, invalidDescription, groupId));
  }

  [Fact]
  public void Create_WithPercentagesBelow100_ShouldThrowException()
  {
    var userId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var expensePartInput = new List<ExpensePartInput>
    {
      new(Guid.NewGuid(), 40),
      new(Guid.NewGuid(), 50)
    };

    Assert.Throws<DomainException>(() => Expense.Create(userId, expensePartInput, 100m, "description", groupId));
  }

  [Fact]
  public void Create_WithPercentagesAbove100_ShouldThrowException()
  {
    var userId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var expensePartInput = new List<ExpensePartInput>
    {
      new(Guid.NewGuid(), 60),
      new(Guid.NewGuid(), 60)
    };

    Assert.Throws<DomainException>(() => Expense.Create(userId, expensePartInput, 100m, "description", groupId));
  }
}