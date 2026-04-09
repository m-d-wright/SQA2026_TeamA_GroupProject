using Bank4Us.Domain.DomainObjects;
using Bank4Us.Domain.Services;
using NSubstitute;
using Reqnroll;
using Reqnroll.Assist;
using System;
using static unit.AccountOpeningTests;

namespace unit
{
    [Binding]
    public class AccountOpeningStepDefinitions
    {
        private ScenarioContext _scenarioContext;

        public AccountOpeningStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given("I am an applicant for a new account with all valid details")]
        public void GivenIAmAnApplicantForANewAccountWithAllValidDetails()
        {
            _scenarioContext["applicant"] = ApplicantFactory.CreateValid();
            _scenarioContext["accountWorkflow"] =  new MockWorkflowFactory().CreateValid();
            
        }


        [When("I submit the application")]
        public void WhenISubmitTheApplication()
        {
            Applicant applicant = (Applicant)_scenarioContext["applicant"];
            IAccountOpeningWorkflow accountWorkFlow = (IAccountOpeningWorkflow)_scenarioContext["accountWorkflow"];
            AccountOpeningAutomation sut = new AccountOpeningAutomation(accountWorkFlow);

            _scenarioContext["applicationResult"] = sut.Run(applicant);
        }

        [Then("I should see confirmation my account has been opened")]
        public void ThenIShouldSeeConfirmationMyAccountHasBeenOpened()
        {

            ProcessResult processResult = (ProcessResult)_scenarioContext["applicationResult"];
            Assert.NotNull(processResult);
            Assert.Empty(processResult.Errors);
            Assert.Equal(ApplicationStatus.Approved, processResult.Status);
        }

        [Given("I am an applicant for a new account with an invalid SSN")]
        public void GivenIAmAnApplicantForANewAccountWithAnInvalidSSN()
        {
            Applicant applicant = new ApplicantBuilder().SetIDNumber("FAKE-SSN-NUM").Build();
            ValidationError invalidIdError = new ValidationError("Invalid SSN format"); // Exact expected from reqs
            IAccountOpeningWorkflow accountWorkFlow = new MockWorkflowFactory().CreateValid();
            accountWorkFlow.ValidateIdentificationNumber(Arg.Any<Applicant>())
                .Returns(new List<ValidationError> { invalidIdError });

            _scenarioContext["accountWorkflow"] = accountWorkFlow;
            _scenarioContext["applicant"] = applicant;
            _scenarioContext["invalidIdError"] = invalidIdError;
        }



        [Then("I should see an error message about the invalid SSN")]
        public void ThenIShouldSeeAnErrorMessageAboutTheInvalidSSN()
        {
            ProcessResult processResult = (ProcessResult)_scenarioContext["applicationResult"];
            ValidationError invalidIdError = (ValidationError)_scenarioContext["invalidIdError"];

            Assert.NotNull(processResult);
            Assert.NotEmpty(processResult.Errors);
            Assert.Equal(invalidIdError, processResult.Errors[0]);
            Assert.Equal(ApplicationStatus.Incomplete, processResult.Status);

        }

        [Given("I am an applicant for a new account with initial deposit of {int}")]
        public void GivenIAmAnApplicantForANewAccountWithInitialDepositOf(int depositAmount)
        {
            ValidationError invalidDepositError = new ValidationError("Invalid opening deposit");
            IAccountOpeningWorkflow accountWorkFlow = new MockWorkflowFactory().CreateValid();

            //I am aware that this is a janky bodge, but it's the only way to keep the structure consistent before we actually implement i
            if (depositAmount < 200) 
                accountWorkFlow.Process(Arg.Any<Applicant>())
                .Returns(new ProcessResult(ApplicationStatus.Cancelled, new List<ValidationError> { invalidDepositError }));
            
            _scenarioContext["accountWorkflow"] = accountWorkFlow;
            _scenarioContext["invalidDepositError"] = invalidDepositError;
            _scenarioContext["applicant"] =
                new ApplicantBuilder().SetOpeningDeposit(depositAmount).Build();

        }

        [Then("I should see an error message about insufficient initial deposit")]
        public void ThenIShouldSeeAnErrorMessageAboutInsufficientInitialDeposit()
        {
            ProcessResult processResult = (ProcessResult)_scenarioContext["applicationResult"];
            ValidationError invalidDepositError = (ValidationError)_scenarioContext["invalidDepositError"];

            Assert.NotNull(processResult);
            Assert.NotEmpty(processResult.Errors);
            Assert.Equal(invalidDepositError, processResult.Errors[0]);
            Assert.Equal(ApplicationStatus.Cancelled, processResult.Status);

        }

    }
}
