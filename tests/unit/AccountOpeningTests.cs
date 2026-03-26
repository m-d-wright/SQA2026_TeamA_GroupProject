
using NSubstitute;
using Bank4Us.AccountOpening;

namespace unit;

public class AccountOpeningTests
{
    [Fact]
    public void EvaluateCitizenship_PermanentResident_StatusNeedsExtraVerification()
    {
        Assert.Equal(0, 1);
    }

    [Fact]
    public void Run_WhenAllVerificationsPass_ReturnsApproved()
    {
        // Arrange
        var workflow = Substitute.For<IAccountOpeningWorkflow>();
        var applicant = ApplicantFactory.CreateValid();
        var sut = new AccountOpeningAutomation(workflow);

        workflow.ValidateIdentificationNumber(applicant).Returns([]);
        workflow.ValidateAddress(applicant).Returns(new ProcessResult(ApplicationStatus.Approved, []));
        workflow.EvaluateCitizenship(applicant).Returns(ApplicationStatus.Approved);
        workflow.Process(applicant).Returns(new ProcessResult(ApplicationStatus.Approved, []));

        // Act
        var result = sut.Run(applicant);

        // Assert
        Assert.Equal(ApplicationStatus.Approved, result.Status);
    }

    [Fact]
    public void Run_WhenDepositBelowMinimum_ReturnsCancelled()
    {
        // Arrange
        var workflow = Substitute.For<IAccountOpeningWorkflow>();
        var applicant = ApplicantFactory.CreateValid() with { OpeningDepositAmount = 199.99m };
        var sut = new AccountOpeningAutomation(workflow);

        workflow.ValidateIdentificationNumber(applicant).Returns([]);
        workflow.ValidateAddress(applicant).Returns(new ProcessResult(ApplicationStatus.Approved, []));
        workflow.EvaluateCitizenship(applicant).Returns(ApplicationStatus.Approved);
        workflow.Process(applicant).Returns(new ProcessResult(ApplicationStatus.Cancelled, []));

        // Act
        var result = sut.Run(applicant);

        // Assert
        Assert.Equal(ApplicationStatus.Cancelled, result.Status);
    }
}
