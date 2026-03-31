using Bank4Us.AccountOpening;
using Bank4Us.Transaction;
using NSubstitute;

namespace unit;

public class TransactionTests
{
    /* Test T6 */
    [Fact]
    public void Transfer_DepositAboveZero_StatusApproved()
    {
        // Arrange
        Transfer transferDeposit = new Transfer(TransferType.Deposit, new Account (), 1);
        // TODO: Replace with MockWorkflowFactory for Green
        ITransferWorkflow transferWorkflow = Substitute.For<ITransferWorkflow>();
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
        Transfer transferDeposit = new Transfer(TransferType.Deposit, new Account(), 0);
        // TODO: Replace with MockWorkflowFactory for Green
        ITransferWorkflow transferWorkflow = Substitute.For<ITransferWorkflow>();
        TransferAutomation transferAutomation = new TransferAutomation(transferWorkflow);

        // Act
        TransferResult transferResult = transferAutomation.InitiateTransfer(transferDeposit);

        // Assert
        Assert.NotNull(transferResult);
        Assert.NotEmpty(transferResult.Errors);
        Assert.Equal(TransferStatus.Cancelled, transferResult.Status);
    }

    /* Test T8 */
    [Fact]
    public void Transfer_ValidWithdrawalAmount_StatusApproved()
    {
        // Arrange
        Transfer transferWithdrawal = new Transfer(TransferType.Withdrawal, new Account { Balance = 200}, 200);
        // TODO: Replace with MockWorkflowFactory for Green
        ITransferWorkflow transferWorkflow = Substitute.For<ITransferWorkflow>();
        TransferAutomation transferAutomation = new TransferAutomation(transferWorkflow);

        // Act
        TransferResult transferResult = transferAutomation.InitiateTransfer(transferWithdrawal);

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
        Transfer transferWithdrawal = new Transfer(TransferType.Withdrawal, new Account { Balance = 200 }, 201);
        // TODO: Replace with MockWorkflowFactory for Green
        ITransferWorkflow transferWorkflow = Substitute.For<ITransferWorkflow>();
        TransferAutomation transferAutomation = new TransferAutomation(transferWorkflow);

        // Act
        TransferResult transferResult = transferAutomation.InitiateTransfer(transferWithdrawal);

        // Assert
        Assert.NotNull(transferResult);
        Assert.NotEmpty(transferResult.Errors);
        Assert.Equal(TransferStatus.Cancelled, transferResult.Status);
    }
}
