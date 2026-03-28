using Bank4Us.AccountOpening;
using NSubstitute;

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

    public class MockWorkflowFactory
    {
        public IAccountOpeningWorkflow CreateValid() 
        {
            IAccountOpeningWorkflow accountWorkFlow = Substitute.For<IAccountOpeningWorkflow>();
            // NOTE: Valid ValidateIDNumber mock unnecessary as it's valid by default
            
            // Valid ValidateAddress
            accountWorkFlow.ValidateAddress(Arg.Any<Applicant>())
                .Returns(new ProcessResult(ApplicationStatus.Approved, 
                new List<ValidationError> { new ValidationError("None") }));
            
            // Valid EvaluateCitizenship mock
            accountWorkFlow.EvaluateCitizenship(Arg.Any<Applicant>())
                .Returns(ApplicationStatus.Approved);

            // Valid process mock
            accountWorkFlow.Process(Arg.Any<Applicant>())
                .Returns(new ProcessResult(ApplicationStatus.Approved, Array.Empty<ValidationError>()));
            
            return accountWorkFlow;
        }
    }


    [Fact]
    public void EvaluateCitizenship_PermanentResident_StatusNeedsExtraVerification()
    {
        Assert.Equal(0,1);
    }
}
