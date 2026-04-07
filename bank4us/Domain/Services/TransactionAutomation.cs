using bank4us.Domain.DomainObjects;

namespace bank4us.Domain.Services
{
    // ---------------------------
    // Workflow Mock Seam (Interface)
    // ---------------------------

    /// <summary>
    /// The domain workflow contract that tests will mock using NSubstitute.
    /// Students do NOT implement this in Lab 2; they only substitute it.
    /// </summary>

    public interface ITransferWorkflow
    {
        TransferResult Deposit(Transfer transfer);
        TransferResult Withdraw(Transfer transfer);
    }


    public sealed class TransferAutomation
    {
        private readonly ITransferWorkflow _workflow;

        public TransferAutomation(ITransferWorkflow workflow)
        {
            _workflow = workflow ?? throw new ArgumentNullException(nameof(workflow));
        }

        public TransferResult InitiateTransfer(Transfer transfer)
        {
            if (transfer.Type == TransferType.Deposit)
            {
                return _workflow.Deposit(transfer);
            }
            else if (transfer.Type == TransferType.Withdraw)
            {
                return _workflow.Withdraw(transfer);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(transfer.Type));
            }
        }
    }
}
