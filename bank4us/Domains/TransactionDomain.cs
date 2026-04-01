using System;
using System.Collections.Generic;

namespace Bank4Us.Transaction
{
    // ---------------------------
    // Domain Models & Enumerations
    // ---------------------------
    public enum TransferStatus
    {
        Approved,
        Cancelled
    }

    public enum TransferType
    {
        Deposit,
        Withdraw
    }

    public enum AccountType
    {
        Checking,
        Savings
    }

    public sealed record AccountHolder(string Name, int Age);

    public sealed class Account
    {
        public AccountType AccountType {get; init;} = AccountType.Checking;

        public AccountHolder? AccountHolder {get; init;}

        public int Balance {get; set;} = 0;
        
    }

    public sealed record Transfer(TransferType Type, Account Account, int amount);

    public sealed record TransferError(string message);

    public sealed record TransferResult(TransferStatus Status, IReadOnlyList<TransferError> Errors);

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
            if (transfer.Type == TransferType.Deposit) {
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