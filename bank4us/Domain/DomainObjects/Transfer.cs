namespace bank4us.Domain.DomainObjects
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

    public sealed record Transfer(TransferType Type, Account Account, int amount);

}
