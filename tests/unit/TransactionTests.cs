using bank4us.Domain.Services;
using bank4us.Domain.DomainObjects;
using NSubstitute;

namespace unit;

public class TransactionTests
{
    /* Test T6 */
    [Fact]
    public void Transfer_DepositAboveZero_StatusApproved()
    {
        // Arrange
        int depositAmount = 1;
        Transfer transferDeposit = new Transfer(TransferType.Deposit, new Account (), depositAmount);
        ITransferWorkflow transferWorkflow = Substitute.For<ITransferWorkflow>();
        transferWorkflow.Deposit(Arg.Any<Transfer>()).Returns(new TransferResult(TransferStatus.Approved,  Array.Empty<TransferError>()));
        
        // SUT
        TransferAutomation transferAutomation = new TransferAutomation(transferWorkflow);

        // Act
        TransferResult transferResult = transferAutomation.InitiateTransfer(transferDeposit);

        // Assert
        Assert.NotNull(transferResult);
        Assert.Empty(transferResult.Errors);
        Assert.Equal(TransferStatus.Approved, transferResult.Status);
    }

    /* Test T7 */
    [Fact]
    public void Transfer_DepositZero_StatusCancelled()
    {
        // Arrange
        int depositAmount = 0;
        TransferError depositZeroError = new TransferError("Invalid deposit amount");
        Transfer transferDeposit = new Transfer(TransferType.Deposit, new Account(), depositAmount);
        ITransferWorkflow transferWorkflow = Substitute.For<ITransferWorkflow>();
        transferWorkflow.Deposit(Arg.Any<Transfer>()).Returns(new TransferResult
        (TransferStatus.Cancelled,  new List<TransferError> { depositZeroError}));
        
        // SUT
        TransferAutomation transferAutomation = new TransferAutomation(transferWorkflow);

        // Act
        TransferResult transferResult = transferAutomation.InitiateTransfer(transferDeposit);

        // Assert
        Assert.NotNull(transferResult);
        Assert.NotEmpty(transferResult.Errors);
        Assert.Equal(depositZeroError, transferResult.Errors[0]);
        Assert.Equal(TransferStatus.Cancelled, transferResult.Status);
    }

    /* Test T8 */
    [Fact]
    public void Transfer_ValidWithdrawalAmount_StatusApproved()
    {
        // Arrange
        Transfer transferWithdraw = new Transfer(TransferType.Withdraw, new Account { Balance = 200}, 200);
        ITransferWorkflow transferWorkflow = Substitute.For<ITransferWorkflow>();
        transferWorkflow.Withdraw(Arg.Any<Transfer>()).Returns(new TransferResult(TransferStatus.Approved,  Array.Empty<TransferError>()));
        
        // SUT
        TransferAutomation transferAutomation = new TransferAutomation(transferWorkflow);

        // Act
        TransferResult transferResult = transferAutomation.InitiateTransfer(transferWithdraw);

        // Assert
        Assert.NotNull(transferResult);
        Assert.Empty(transferResult.Errors);
        Assert.Equal(TransferStatus.Approved, transferResult.Status);
    }

    /* Test T9 */
    [Fact]
    public void Transfer_InvalidWithdrawalAmount_StatusCancelled()
    {
        // Arrange
        int withdrawalAmount = 201;
        int currBalance = 200;
        TransferError overdraftError = new TransferError("Invalid withdrawal amount, not enough funds");
        Transfer transferWithdraw = new Transfer(TransferType.Withdraw, new Account { Balance = currBalance }, withdrawalAmount);
        ITransferWorkflow transferWorkflow = Substitute.For<ITransferWorkflow>();
        transferWorkflow.Withdraw(Arg.Any<Transfer>()).Returns(new TransferResult(TransferStatus.Cancelled,  new List<TransferError> {overdraftError} ));
        
        // SUT
        TransferAutomation transferAutomation = new TransferAutomation(transferWorkflow);

        // Act
        TransferResult transferResult = transferAutomation.InitiateTransfer(transferWithdraw);

        // Assert
        Assert.NotNull(transferResult);
        Assert.NotEmpty(transferResult.Errors);
        Assert.Equal(overdraftError, transferResult.Errors[0]);
        Assert.Equal(TransferStatus.Cancelled, transferResult.Status);
    }
}
