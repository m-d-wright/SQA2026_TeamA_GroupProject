namespace Bank4Us.Domain.DomainObjects
{

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

    public sealed class Transfer
    {
        const int UNSET_TRANSFER_ID = -1;
        public int Id { get; set; }
        public TransferType Type { get; set; }
        public Account? Account { get; set; }
        public int Amount { get; set; }

        public Transfer() { }

        public Transfer(TransferType transferType, Account? account, int amount)
        {
            Type = transferType; 
            Account = account;
            Amount = amount;
        }
    }
}
