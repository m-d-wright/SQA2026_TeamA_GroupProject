using Bank4Us.Domain.DomainObjects;

namespace Bank4Us.Domain.Services
{
    // ---------------------------
    // Workflow Mock Seam (Interface)
    // ---------------------------

    /// <summary>
    /// The domain workflow contract that tests will mock using NSubstitute.
    /// </summary>
    public interface IAccountOpeningWorkflow
    {
        /// <summary>
        /// Returns validation errors related to government identification.
        /// Empty list means ID is syntactically/structurally valid.
        /// </summary>
        IReadOnlyList<ValidationError> ValidateIdentificationNumber(Applicant applicant);

        /// <summary>
        /// Validates the postal address and returns a ProcessResult.
        /// Approved indicates the address is acceptable; otherwise contains errors/status.
        /// </summary>
        ProcessResult ValidateAddress(Applicant applicant);

        /// <summary>
        /// Evaluates citizenship/residency signals and returns a status that may
        /// require further verification steps before proceeding to account creation.
        /// </summary>
        ApplicationStatus EvaluateCitizenship(Applicant applicant);

        /// <summary>
        /// Performs the final step of the workflow (e.g., account creation, persistence)
        /// and returns the terminal ProcessResult.
        /// </summary>
        ProcessResult Process(Applicant applicant);
    }

    // ---------------------------
    // System Under Test (SUT)
    // ---------------------------

    /// <summary>
    /// The orchestration runner.
    /// Tests will mock IAccountOpeningWorkflow and verify interactions and outcomes.
    /// </summary>
    public sealed class AccountOpeningAutomation
    {
        private readonly IAccountOpeningWorkflow _workflow;

        public AccountOpeningAutomation(IAccountOpeningWorkflow workflow)
        {
            _workflow = workflow ?? throw new ArgumentNullException(nameof(workflow));
        }

        /// <summary>
        /// Runs the account opening orchestration by delegating to the workflow.
        /// The order of calls is intentional to support interaction verification in tests:
        /// 1) ValidateIdentificationNumber
        /// 2) ValidateAddress
        /// 3) EvaluateCitizenship
        /// 4) Process
        /// </summary>
        public ProcessResult Run(Applicant applicant)
        {
            // 1) ID validation short-circuits if there are errors
            var idErrors = _workflow.ValidateIdentificationNumber(applicant);
            if (idErrors.Count > 0)
            {
                return new ProcessResult(ApplicationStatus.Incomplete, idErrors);
            }

            // 2) Address validation short-circuits unless it's Approved
            var addressResult = _workflow.ValidateAddress(applicant);
            if (addressResult.Status != ApplicationStatus.Approved)
            {
                return addressResult;
            }

            // 3) Citizenship evaluation may require extra/pending verification
            var status = _workflow.EvaluateCitizenship(applicant);
            if (status == ApplicationStatus.NeedsExtraVerification ||
                status == ApplicationStatus.PendingVerification)
            {
                return new ProcessResult(status, Array.Empty<ValidationError>());
            }

            // 4) Final processing (happy path) returns the terminal result
            return _workflow.Process(applicant);
        }
    }
}
