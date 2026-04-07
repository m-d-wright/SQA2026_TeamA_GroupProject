using Bank4Us.Domain.DomainObjects;
using Bank4Us.Domain.Services;
using NSubstitute;
using Reqnroll;
using System;

namespace unit
{
    [Binding]
    public class TransactionsStepDefinitions
    {

        private ScenarioContext _scenarioContext;

        public TransactionsStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }


        [Given("I want to deposit a positive amount into my account")]
        public void GivenIWantToDepositAPositiveAmountIntoMyAccount()
        {
            int depositAmount = 1;
            Transfer transferDeposit = new Transfer(TransferType.Deposit, new Account(), depositAmount);
            ITransferWorkflow transferWorkflow = Substitute.For<ITransferWorkflow>();
            transferWorkflow.Deposit(Arg.Any<Transfer>()).Returns(new TransferResult(TransferStatus.Approved, Array.Empty<TransferError>()));

            _scenarioContext["transferWorkflow"] = transferWorkflow;
            _scenarioContext["transferDeposit"] = transferDeposit;
        }

        [When("I make the deposit")]
        public void WhenIMakeTheDeposit()
        {
            ITransferWorkflow transferWorkflow = (ITransferWorkflow)_scenarioContext["transferWorkflow"];
            Transfer transferDeposit = (Transfer)_scenarioContext["transferDeposit"];

            TransferAutomation transferAutomation = new TransferAutomation(transferWorkflow);
            TransferResult transferResult = transferAutomation.InitiateTransfer(transferDeposit);

            _scenarioContext["transferResult"] = transferResult;
        }

        [Then("The transfer should be approved")]
        public void ThenTheTransferShouldBeApproved()
        {
            TransferResult transferResult = (TransferResult)_scenarioContext["transferResult"];

            Assert.NotNull(transferResult);
            Assert.Empty(transferResult.Errors);
            Assert.Equal(TransferStatus.Approved, transferResult.Status);

        }

        [Given("I want to deposit a zero amount into my account")]
        public void GivenIWantToDepositAZeroAmountIntoMyAccount()
        {
            int depositAmount = 0;
            TransferError depositZeroError = new TransferError("Invalid deposit amount");
            Transfer transferDeposit = new Transfer(TransferType.Deposit, new Account(), depositAmount);
            ITransferWorkflow transferWorkflow = Substitute.For<ITransferWorkflow>();
            transferWorkflow.Deposit(Arg.Any<Transfer>()).Returns(new TransferResult
            (TransferStatus.Cancelled, new List<TransferError> { depositZeroError }));

            _scenarioContext["depositZeroError"] = depositZeroError;
            _scenarioContext["transferWorkflow"] = transferWorkflow;
            _scenarioContext["transferDeposit"] = transferDeposit;
        }

        [Then("The transfer should be cancelled")]
        public void ThenTheTransferShouldBeCancelled()
        {
            TransferResult transferResult = (TransferResult)_scenarioContext["transferResult"];
            TransferError depositZeroError = (TransferError)_scenarioContext["depositZeroError"];

            Assert.NotNull(transferResult);
            Assert.NotEmpty(transferResult.Errors);
            Assert.Equal(depositZeroError, transferResult.Errors[0]);
            Assert.Equal(TransferStatus.Cancelled, transferResult.Status);


        }

        [Given("I have an account with Bank4Us with a balance of {int}")]
        public void GivenIHaveAnAccountWithBank4UsWithABalanceOf(int balance)
        {
            _scenarioContext["account"] = new Account { Balance = balance };
        }

        [Given("I want to withdraw an amount less than or equal to my balance")]
        public void GivenIWantToWithdrawAnAmountLessThanOrEqualToMyBalance()
        {
            Account account = (Account)_scenarioContext["account"];

            Transfer transferWithdraw = new Transfer(TransferType.Withdraw, account, 200);
            ITransferWorkflow transferWorkflow = Substitute.For<ITransferWorkflow>();
            transferWorkflow.Withdraw(Arg.Any<Transfer>()).Returns(new TransferResult(TransferStatus.Approved, Array.Empty<TransferError>()));

            _scenarioContext["transferWorkflow"] = transferWorkflow;
            _scenarioContext["transferWithdraw"] = transferWithdraw;
        }

        [When("I make the withdrawl")]
        public void WhenIMakeTheWithdrawl()
        {
            ITransferWorkflow transferWorkflow = (ITransferWorkflow)_scenarioContext["transferWorkflow"];
            Transfer transferWithdraw = (Transfer)_scenarioContext["transferWithdraw"];

            TransferAutomation transferAutomation = new TransferAutomation(transferWorkflow);
            TransferResult transferResult = transferAutomation.InitiateTransfer(transferWithdraw);

            _scenarioContext["transferResult"] = transferResult;
        }

        [Then("The withdrawl should be approved")]
        public void ThenTheWithdrawlShouldBeApproved()
        {
            TransferResult transferResult = (TransferResult)_scenarioContext["transferResult"];

            Assert.NotNull(transferResult);
            Assert.Empty(transferResult.Errors);
            Assert.Equal(TransferStatus.Approved, transferResult.Status);
        }

        [Given("I want to withdraw an amount greater than my balance")]
        public void GivenIWantToWithdrawAnAmountGreaterThanMyBalance()
        {
            Account account = (Account)_scenarioContext["account"];
            TransferError overdraftError = new TransferError("Invalid withdrawal amount, not enough funds");

            Transfer transferWithdraw = new Transfer(TransferType.Withdraw, account, 201);
            ITransferWorkflow transferWorkflow = Substitute.For<ITransferWorkflow>();
            transferWorkflow.Withdraw(Arg.Any<Transfer>()).Returns(
                new TransferResult(TransferStatus.Cancelled, new List<TransferError> { overdraftError }));

            _scenarioContext["transferWorkflow"] = transferWorkflow;
            _scenarioContext["transferWithdraw"] = transferWithdraw;
            _scenarioContext["overdraftError"] = overdraftError;
        }

        [Then("The withdrawl should be cancelled")]
        public void ThenTheWithdrawlShouldBeCancelled()
        {
            TransferResult transferResult = (TransferResult)_scenarioContext["transferResult"];
            TransferError overdraftError = (TransferError)_scenarioContext["overdraftError"];

            Assert.NotNull(transferResult);
            Assert.NotEmpty(transferResult.Errors);
            Assert.Equal(TransferStatus.Cancelled, transferResult.Status);
        }


    }
}
