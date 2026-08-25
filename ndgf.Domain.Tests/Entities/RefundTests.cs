using ndgf.Domain.Entities;
using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Tests.Entities;

public class RefundTests
{
  [Fact]
  public void Create_WithValidData_ShouldCreateRefund()
  {
    var amount = 100m;
    var description = "description";
    var payerId = Guid.NewGuid();
    var receiverId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    
    var refund = Refund.Create(payerId, receiverId, amount, description, groupId );
    
    Assert.Equal(payerId, refund.PayerId);
    Assert.Equal(receiverId, refund.ReceiverId);
    Assert.Equal(amount, refund.Amount);
    Assert.Equal(description, refund.Description);
    Assert.Equal(groupId, refund.GroupId);
    Assert.True(refund.CreatedAt <= DateTime.UtcNow);
  }
  
  [Fact]
  public void Create_WithInvalidPayerId_ShouldThrowException()
  {
    var amount = 100m;
    var description = "description";
    var invalidPayerId = Guid.Empty;
    var receiverId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    
    Assert.Throws<DomainException>(() => Refund.Create(invalidPayerId, receiverId, amount, description, groupId));
  }

  [Fact]
  public void Create_WithInvalidReceiverId_ShouldThrowException()
  {
    var amount = 100m;
    var description = "description";
    var payerId = Guid.NewGuid();
    var invalidReceiverId = Guid.Empty;
    var groupId = Guid.NewGuid();
    
    Assert.Throws<DomainException>(() => Refund.Create(payerId, invalidReceiverId, amount, description, groupId));
  }

  [Fact]
  public void Create_WithInvalidAmount_ShouldThrowException()
  {
    var invalidAmount = 0;
    var description = "description";
    var payerId = Guid.NewGuid();
    var receiverId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    
    Assert.Throws<DomainException>(() => Refund.Create(payerId, receiverId, invalidAmount, description, groupId));
  }

  [Fact]
  public void Create_WithInvalidGroupId_ShouldThrowException()
  {
    var amount = 100m;
    var description = "description";
    var payerId = Guid.NewGuid();
    var receiverId = Guid.NewGuid();
    var invalidGroupId = Guid.Empty;
    
    Assert.Throws<DomainException>(() => Refund.Create(payerId, receiverId, amount, description, invalidGroupId));
  }
  
  [Fact]
  public void Create_WithValidDataWithoutDescription_ShouldCreateRefund()
  {
    var amount = 100m;
    var payerId = Guid.NewGuid();
    var receiverId = Guid.NewGuid();
    var groupId = Guid.NewGuid();

    var refund = Refund.Create(payerId, receiverId, amount, null, groupId);
    
    Assert.Equal(payerId, refund.PayerId);
    Assert.Equal(receiverId, refund.ReceiverId);
    Assert.Equal(amount, refund.Amount);
    Assert.Equal(groupId, refund.GroupId);
    Assert.True(refund.CreatedAt <= DateTime.UtcNow);
    Assert.Null(refund.Description);
  }
  
  [Fact]
  public void Create_WithPayerIdEqualToReceiverId_ShouldThrowException()
  {
    var amount = 100m;
    var payerId = Guid.NewGuid();
    var receiverId = payerId;
    var groupId = Guid.NewGuid();
    var description = "description";
    
    Assert.Throws<DomainException>(() => Refund.Create(payerId, receiverId, amount, description, groupId));
  }
}