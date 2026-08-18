using ndgf.Domain.Entities;
using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Tests.Entities;

public class ExpensePartTests
{
  [Fact]

  public void CreateWithValidData_ShouldCreateExpensePart()
  {
    //Arrange
    
    var userId = Guid.NewGuid();
    var expenseId = Guid.NewGuid();
    var percentage = 25;
    
    //Act
    
    var expensePart = ExpensePart.Create(userId, expenseId, percentage);
    
    //Assert
    
    Assert.Equal(userId, expensePart.UserId);
    Assert.Equal(expenseId, expensePart.ExpenseId);
    Assert.True(expensePart.Percentage == percentage);
    Assert.True(expensePart.Percentage <= 100);
    Assert.True(expensePart.Percentage >= 0);
    Assert.NotEqual(Guid.Empty, expensePart.ExpenseId);
    Assert.NotEqual(Guid.Empty, expensePart.UserId);
  }

  [Fact]
  public void CreateWithTooHighPercentage_ShouldThrowException()
  {
    var userId = Guid.NewGuid();
    var expenseId = Guid.NewGuid();
    var percentage = 105;
    
    Assert.Throws<DomainException>(() => ExpensePart.Create(userId, expenseId, percentage));
  }

  [Fact]
  public void CreateWithNegativePercentage_ShouldThrowException()
  {
    var userId = Guid.NewGuid();
    var expenseId = Guid.NewGuid();
    var percentage = -15;
    
    Assert.Throws<DomainException>(() => ExpensePart.Create(userId, expenseId, percentage));
  }

  [Fact]
  public void CreateWithZeroPercentage_ShouldCreateExpensePart()
  {
    var userId = Guid.NewGuid();
    var expenseId = Guid.NewGuid();
    var percentage = 0;
    
    var expensePart = ExpensePart.Create(userId, expenseId, percentage);
    
    Assert.Equal(userId,  expensePart.UserId);
    Assert.Equal(expenseId, expensePart.ExpenseId);
    Assert.True(expensePart.Percentage == percentage);
    Assert.True(expensePart.Percentage == 0);
    Assert.NotEqual(Guid.Empty, expensePart.ExpenseId);
    Assert.NotEqual(Guid.Empty, expensePart.UserId);
  }

  [Fact]
  public void CreateWithHundredPercentage_ShouldCreateExpensePart()
  {
    var userId = Guid.NewGuid();
    var expenseId = Guid.NewGuid();
    var percentage = 100;
    
    var expensePart = ExpensePart.Create(userId, expenseId, percentage);
    
    Assert.Equal(userId, expensePart.UserId);
    Assert.Equal(expenseId, expensePart.ExpenseId);
    Assert.True(expensePart.Percentage == percentage);
    Assert.True(expensePart.Percentage == 100);
    Assert.NotEqual(Guid.Empty, expensePart.ExpenseId);
    Assert.NotEqual(Guid.Empty, expensePart.UserId);
  }
  
  [Fact]
  public void CreateWIthInvalidUserId_ShouldThrowException()
  {
    var userId = Guid.Empty;
    var expenseId = Guid.NewGuid();
    var percentage = 25;
    
    Assert.Throws<DomainException>(() => ExpensePart.Create(userId, expenseId, percentage));
  }

  [Fact]
  public void CreateWIthInvalidExpenseId_ShouldThrowException()
  {
    var userId = Guid.NewGuid();
    var expenseId = Guid.Empty;
    var percentage = 25;
    
    Assert.Throws<DomainException>(() => ExpensePart.Create(userId, expenseId, percentage));
  }
}