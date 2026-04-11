using Bank4Us.Domain.DomainObjects;
using Bank4Us.Domain.Repositories;
using Bank4Us.Domain.Services;
using Bank4Us.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using static unit.AccountOpeningTests;

namespace integration;

public class IntegrationTests
{
    private static BankingContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<BankingContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        var ctx = new BankingContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    /* Integration Test 0 - I0 */
    [Fact]
    public void Deposit_ValidTransfer_PersistentTransfer()
    {
        // Use a unique database name to isolate this test run
        string dbName = "BankingDb_" + System.Guid.NewGuid();
        BankingContext ctx = CreateContext(dbName);

        // Arrange
        int transferId = default;
        TransferRepository transferRepository = new TransferRepository();

        // initialize data
        Account account = new Account
        {
            AccountType = AccountType.Checking,
            AccountHolder = new AccountHolder("123-45-678", IdentifierType.SSN),
            Balance = 1500
        };
        Transfer expectedTransfer = new Transfer
        {
            Account = account,
            Type = TransferType.Deposit,
            Amount = 100
        };

        ITransactionWorkflow transferWorkflow = Substitute.For<ITransactionWorkflow>();
        transferWorkflow.Deposit(Arg.Any<Transfer>()).Returns(ci =>
        {
            Transfer actualTransfer = (Transfer)ci[0];
            transferId = transferRepository.CreateTransfer(actualTransfer, ctx);
            return new TransferResult(TransferStatus.Approved, Array.Empty<TransferError>());  
        });

        // SUT
        TransactionAutomation transactionAutomation = new TransactionAutomation(transferWorkflow);

        // Act
        TransferResult transferResult = transactionAutomation.InitiateTransfer(expectedTransfer);

        // Get the transfer back
        Transfer? actualTransfer = transferRepository.GetTransferById(transferId, ctx);

        // Assert
        Assert.Equal(TransferStatus.Approved, transferResult.Status);
        Assert.NotNull(actualTransfer);
        Assert.Equal(expectedTransfer.Amount, actualTransfer.Amount);
        Assert.Equal(expectedTransfer.Type, actualTransfer.Type);
        Assert.Equal(expectedTransfer.Account, actualTransfer.Account);


    }


    /* Integration Test 1 - I1 */
    [Fact]
    public void Run_ValidApplicant_AccountOpenedWithMatchingData()
    {
        // Use a unique database name to isolate this test run
        string dbName = "BankingDb_" + System.Guid.NewGuid();
        BankingContext ctx = CreateContext(dbName);

        // Arrange
        int accountId = default;
        IAccountOpeningWorkflow accountWorkFlow = new MockWorkflowFactory().CreateValid();
        Applicant applicant = ApplicantFactory.CreateValid();
        AccountRepository accountRepository = new AccountRepository();

        // initialize data
        Account expectedAccount = new Account
        {
            AccountType = AccountType.Checking,
            AccountHolder = new AccountHolder(applicant.IdentificationNumber, applicant.IdentifierType),
            Balance = 1500
        };

        accountWorkFlow.Process(Arg.Any<Applicant>()).Returns(ci =>
        {
            
            accountId = accountRepository.CreateAccount(expectedAccount, ctx);
            return new ProcessResult(ApplicationStatus.Approved, Array.Empty<ValidationError>());
        });

        // SUT
        AccountOpeningAutomation sut = new AccountOpeningAutomation(accountWorkFlow);

        // Act
        ProcessResult processResult = sut.Run(applicant);

        // Get the transfer back
        Account? actualAccount = accountRepository.GetAccountById(accountId, ctx);

        // Assert
        Assert.NotNull(actualAccount);
        Assert.Equal(ApplicationStatus.Approved, processResult.Status);
        Assert.Equal(expectedAccount.AccountHolder.IdentificationNumber, actualAccount.AccountHolder.IdentificationNumber);
        Assert.Equal(expectedAccount.AccountHolder.IdentifierType, actualAccount.AccountHolder.IdentifierType);


    }

    
}
