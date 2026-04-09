using NSubstitute;
using Bank4Us.Domain.DomainObjects;
using Bank4Us.Domain.Services;
using NSubstitute.Core;

namespace unit;

public class AccountOpeningTests
{

    /* Builder class for applicant */
    public class ApplicantBuilder
    {
        private IdentifierType _idType = IdentifierType.SSN;
        private string _idNumber = "123-45-6789";
        private Address _address = new Address("123 Main St", "Milwaukee", "WI", "53202");
        private CitizenshipStatus _citizenStatus = CitizenshipStatus.Citizen;
        private decimal _openingDeposit = 200m;
        private bool _hasResidencyDoc = true;

        public ApplicantBuilder SetIDType(IdentifierType idType)
        {
            _idType = idType;
            return this;
        }

        public ApplicantBuilder SetIDNumber(string idNumber)
        {
            _idNumber = idNumber;
            return this;
        }
        public ApplicantBuilder SetAddress(Address address)
        {
            _address = address;
            return this;
        }
        public ApplicantBuilder SetCitizenStatus(CitizenshipStatus citizenStatus)
        {
            _citizenStatus = citizenStatus;
            return this;
        }
        public ApplicantBuilder SetOpeningDeposit(decimal openingDeposit)
        {
            _openingDeposit = openingDeposit;
            return this;
        }
        public ApplicantBuilder SetResidencyDoc(bool hasResidencyDoc)
        {
            _hasResidencyDoc = hasResidencyDoc;
            return this;
        }

        public Applicant Build()
        {
            return new Applicant 
            {
               IdentifierType = _idType, 
               IdentificationNumber = _idNumber, 
               Address = _address, 
               CitizenshipStatus = _citizenStatus, 
               OpeningDepositAmount = _openingDeposit, 
               HasResidencyDocument = _hasResidencyDoc 
            };
        }

    }

    /* Applicant factory */
    public static class ApplicantFactory
    {
        /// <summary>
        /// Provides a canonical, valid applicant for tests.
        /// Tests can override individual properties to drive specific scenarios.
        /// </summary>
        public static Applicant CreateValid() => new()
        {
            IdentifierType = IdentifierType.SSN,
            IdentificationNumber = "123-45-6789",
            Address = new Address("123 Main St", "Milwaukee", "WI", "53202"),
            CitizenshipStatus = CitizenshipStatus.Citizen,
            OpeningDepositAmount = 200m,
            HasResidencyDocument = true
        };
    }


    /* Workflow factory for creating Workflows */
    public class MockWorkflowFactory
    {
        public IAccountOpeningWorkflow CreateValid() 
        {
            IAccountOpeningWorkflow accountWorkFlow = Substitute.For<IAccountOpeningWorkflow>();
            // NOTE: Valid ValidateIDNumber mock unnecessary as it's valid by default
            
            // Valid ValidateAddress
            accountWorkFlow.ValidateAddress(Arg.Any<Applicant>())
                .Returns(new ProcessResult(ApplicationStatus.Approved,
                Array.Empty<ValidationError>()));
            
            // Valid EvaluateCitizenship mock
            accountWorkFlow.EvaluateCitizenship(Arg.Any<Applicant>())
                .Returns(ApplicationStatus.Approved);

            // Valid process mock
            accountWorkFlow.Process(Arg.Any<Applicant>())
                .Returns(new ProcessResult(ApplicationStatus.Approved, Array.Empty<ValidationError>()));
            
            return accountWorkFlow;
        }
    }

    /* Test T1 */
    [Fact]
    public void ValidateIdentificationNumber_ValidSSN_StatusApproved()
    {
        // Arrange
        IAccountOpeningWorkflow accountWorkFlow = new MockWorkflowFactory().CreateValid();
        Applicant applicant = ApplicantFactory.CreateValid();
        // SUT
        AccountOpeningAutomation sut = new AccountOpeningAutomation(accountWorkFlow);

        // Act
        ProcessResult processResult = sut.Run(applicant);

        // Assert - Expected SUT behavior
        Assert.NotNull(processResult);
        Assert.Empty(processResult.Errors);
        Assert.Equal(ApplicationStatus.Approved, processResult.Status);

        // Assert - Mocked objects were involved/not involved
        accountWorkFlow.Received(1).Process(applicant);
    }

    /* Test T2 */
    [Fact]
    public void ValidateIdentificationNumber_InvalidSSN_ValidationErrorInvalidFormat()
    {

        // Arrange
        ValidationError invalidIdError = new ValidationError("Invalid SSN format"); // Exact expected from reqs
        Applicant applicant = new ApplicantBuilder().SetIDNumber("FAKE-SSN-NUM").Build();
        IAccountOpeningWorkflow accountWorkFlow = new MockWorkflowFactory().CreateValid();
        accountWorkFlow.ValidateIdentificationNumber(Arg.Any<Applicant>())
            .Returns(new List<ValidationError> { invalidIdError });
            
        // SUT
        AccountOpeningAutomation sut = new AccountOpeningAutomation(accountWorkFlow);

        // Act
        ProcessResult processResult = sut.Run(applicant);

        // Assert - Expected SUT behavior
        Assert.NotNull(processResult);
        Assert.NotEmpty(processResult.Errors);
        Assert.Equal(invalidIdError, processResult.Errors[0]);
        Assert.Equal(ApplicationStatus.Incomplete, processResult.Status);

        // Assert - Mocked objects were involved/not involved
        accountWorkFlow.Received(1).ValidateIdentificationNumber(applicant);
        accountWorkFlow.DidNotReceiveWithAnyArgs().ValidateAddress(Arg.Any<Applicant>());
        accountWorkFlow.DidNotReceiveWithAnyArgs().EvaluateCitizenship(Arg.Any<Applicant>());
        accountWorkFlow.DidNotReceiveWithAnyArgs().Process(Arg.Any<Applicant>());

    }

    /* Test T3 and Test T4 */
    [Theory]
    [InlineData(200)]
    [InlineData(201)]
    public void Process_OpeningDepositGreaterEqual200_StatusApproved(int depositAmount)
    {
        // Arrange
        Applicant applicant = new ApplicantBuilder().SetOpeningDeposit(depositAmount).Build();
        IAccountOpeningWorkflow accountWorkFlow = new MockWorkflowFactory().CreateValid();

        // SUT
        AccountOpeningAutomation sut = new AccountOpeningAutomation(accountWorkFlow);

        // Act
        ProcessResult processResult = sut.Run(applicant);

        //Assert
        Assert.NotNull(processResult);
        Assert.Empty(processResult.Errors);
        Assert.Equal(ApplicationStatus.Approved, processResult.Status);

        // Assert - Mocked objects were involved/not involved
        accountWorkFlow.Received(1).Process(applicant);
    }

    /* Test T5*/
    [Fact]
    public void Process_OpeningDepositBelow200_StatusCancelled()
    {
        // Arrange
        int depositAmount = 199; // BVA
        ValidationError invalidDepositError = new ValidationError("Invalid opening deposit");
        Applicant applicant = new ApplicantBuilder().SetOpeningDeposit(depositAmount).Build();
        IAccountOpeningWorkflow accountWorkFlow = new MockWorkflowFactory().CreateValid();
        accountWorkFlow.Process(Arg.Any<Applicant>())
            .Returns(new ProcessResult(ApplicationStatus.Cancelled, new List<ValidationError> { invalidDepositError }));

        // SUT
        AccountOpeningAutomation sut = new AccountOpeningAutomation(accountWorkFlow);

        // Act
        ProcessResult processResult = sut.Run(applicant);

        //Assert
        Assert.NotNull(processResult);
        Assert.NotEmpty(processResult.Errors);
        Assert.Equal(invalidDepositError, processResult.Errors[0]);
        Assert.Equal(ApplicationStatus.Cancelled, processResult.Status);

        // Assert - Mocked objects were involved/not involved
        accountWorkFlow.Received(1).Process(applicant);
    }

    /* Mutation killing tests */

    /* Test T10 */
    [Fact]
    public void AccountOpeningAutomation_WorkflowIsNull_ThrowsArgumentNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => new AccountOpeningAutomation(workflow: null));
        Assert.Equal("workflow", ex.ParamName);
        
    }

}
